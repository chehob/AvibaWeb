namespace AvibaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MDeskBalanceView : DbMigration
    {
        public override void Up()
        {
            const string script =
            @"
            CREATE VIEW dbo.VDeskBalances AS 
            SELECT NEWID() AS Id, cte.DeskID AS DeskId, cte.Description AS DeskName, ISNULL( cte.Balance, 0 ) + ISNULL( SUM( q.Cash ), 0 ) Balance
            FROM
            (
                SELECT ds.OperationDateTime, d.Description, ds.DeskID, ds.Balance,
                        ROW_NUMBER() OVER (PARTITION BY DeskID ORDER BY OperationDateTime DESC) AS rn
                FROM [BookingDB].dbo.DeskSessions ds
                JOIN [BookingDB].dbo.Desks d ON ds.DeskID = d.ID
            ) cte
            OUTER APPLY
            (
	            SELECT
		            Cash = 
			            CASE 
				            WHEN tio.OperationTypeID = 1 THEN ISNULL( cap.Amount, 0 )
				            WHEN tio.OperationTypeID IN ( 2, 5, 6 ) AND cap.Amount IS NOT NULL AND cap.Amount <> 0 THEN ISNULL( -cap.Amount, 0 ) + ISNULL( -p.PenaltySum, 0 )
				            WHEN tio.OperationTypeID IN ( 2, 5, 6 ) AND ( cap.Amount IS NULL OR cap.Amount = 0 ) THEN ISNULL( -cap.Amount, 0 )
				            WHEN tio.OperationTypeID IN ( 3, 7 ) THEN ISNULL( chdcap.Amount, 0 )
			            END
	            FROM [BookingDB].dbo.TicketOperations tio
	            JOIN [BookingDB].dbo.Tickets t ON tio.TicketID = t.ID
		            LEFT OUTER JOIN [BookingDB].dbo.Tickets chd ON tio.TicketID = chd.Parent
	            LEFT OUTER JOIN
		            ( 
			            SELECT COUNT( p.BSONumber ) PenaltyCount, SUM( p.Amount ) PenaltySum, s.TicketID 
			            FROM [BookingDB].dbo.Penalties p
			            JOIN [BookingDB].dbo.Segments s ON s.ID = p.SegmentID
			            GROUP BY s.TicketID
		            ) p ON p.TicketID = tio.TicketID
	            LEFT OUTER JOIN 
		            (
			            SELECT TicketID, OperationTypeID, SUM( Amount ) Amount
			            FROM [BookingDB].dbo.Payments
			            WHERE PaymentType IN ( 'CA', 'ÍÀ' )
			            GROUP BY TicketID, OperationTypeID
		            ) cap ON tio.TicketID = cap.TicketID AND
		            ( ( tio.OperationTypeID = 1 AND cap.OperationTypeID = 1 ) OR
			            ( tio.OperationTypeID IN ( 2, 6 ) AND cap.OperationTypeID = 2 ) OR
			            ( tio.OperationTypeID = 8 AND cap.OperationTypeID = 8 ) OR
			            ( tio.OperationTypeID = 5 AND cap.OperationTypeID = 1 ) )
	            LEFT OUTER JOIN
		            (
			            SELECT TicketID, OperationTypeID, SUM( Amount ) Amount
			            FROM [BookingDB].dbo.Payments
			            WHERE PaymentType IN ( 'CA', 'ÍÀ', 'EX' )
			            GROUP BY TicketID, OperationTypeID
		            ) chdcap ON chdcap.TicketID = chd.ID AND
			            ( tio.OperationTypeID IN ( 3, 7 ) AND chdcap.OperationTypeID = 3 )
	            WHERE tio.ExecutionDateTime > cte.OperationDateTime
		            AND tio.DeskID = cte.DeskID
		            AND ( ( tio.OperationTypeID = 1 AND t.Parent IS NULL ) OR ( tio.OperationTypeID IN ( 2, 3, 5, 6, 7, 8 ) ) )
		            AND ( ( tio.OperationTypeID <> 5 AND NOT EXISTS 
				            ( SELECT ID
				            FROM [BookingDB].dbo.TicketOperations 
				            WHERE TicketID = tio.TicketID AND OperationTypeID = 5 
				            AND DeskID = tio.DeskID ) )
				            OR ( tio.OperationTypeID = 5 AND NOT EXISTS 
				            ( SELECT ID
				            FROM [BookingDB].dbo.TicketOperations 
				            WHERE TicketID = tio.TicketID AND OperationTypeID <> 5 
				            AND DeskID = tio.DeskID ) ) )
	            UNION ALL
	            SELECT 
		            SUM( li.Rate * li.Amount ) Cash
	            FROM [BookingDB].dbo.Luggage l
	            JOIN [BookingDB].dbo.LuggageOperations lo ON l.LuggageID = lo.LuggageID AND lo.OperationTypeID = 1
		            JOIN [BookingDB].dbo.LuggageItems li ON l.LuggageID = li.LuggageID
	            WHERE lo.DeskID = cte.DeskID AND
		            lo.OperationDateTime > cte.OperationDateTime
	            UNION ALL
	            SELECT 
		            -SUM( li.Rate * li.Amount ) Cash
	            FROM [BookingDB].dbo.Luggage l
	            JOIN [BookingDB].dbo.LuggageOperations lo ON l.LuggageID = lo.LuggageID AND lo.OperationTypeID = 5
		            JOIN [BookingDB].dbo.LuggageItems li ON l.LuggageID = li.LuggageID
	            WHERE lo.DeskID = cte.DeskID AND
		            lo.OperationDateTime > cte.OperationDateTime
		            AND l.IsCancelled = 1
	            UNION ALL
	            SELECT 
		            Cash = 
			            CASE
				            WHEN kt.IsCanceled = 1 THEN -kt.Amount
				            ELSE kt.Amount
			            END
	            FROM [BookingDB].dbo.KRSs k 
	            JOIN [BookingDB].dbo.KRSTaxes kt ON k.ID = kt.KRSID
	            WHERE 
		            k.KRSSerie <> 'ÒÊÏ'
		            AND k.OperationTypeID NOT IN ( 11, 12, 15, 19, 21 )
		            AND ( 
			            ( k.DateCreated > cte.OperationDateTime AND ( ( k.DeskID = cte.DeskID ) ) ) OR 
			            ( k.DateCanceled > cte.OperationDateTime AND ( ( k.CancelDeskID = cte.DeskID ) ) AND ( ( k.OperationTypeID = 3 OR ( k.OperationTypeID <> 1 ) ) OR k.OperationTypeID = 1 ) ) )
		            AND ( k.TypeID IS NULL OR k.TypeID = 1 )
	            UNION ALL
	            SELECT
		            lf.Amount Cash
	            FROM [BookingDB].dbo.LuggageFees lf
	            LEFT OUTER JOIN [BookingDB].dbo.LuggageItems li ON lf.LuggageItemID = li.ItemID
		            LEFT OUTER JOIN [BookingDB].dbo.Luggage l ON li.LuggageID = l.LuggageID
			            LEFT OUTER JOIN [BookingDB].dbo.LuggageOperations lo ON l.LuggageID = lo.LuggageID
	            WHERE
		            lo.DeskID = cte.DeskID
		            AND lf.DateCreated > cte.OperationDateTime
	            UNION ALL
	            SELECT
		            -lf.Amount Cash
	            FROM [BookingDB].dbo.LuggageFees lf
	            LEFT OUTER JOIN [BookingDB].dbo.LuggageItems li ON lf.LuggageItemID = li.ItemID
		            LEFT OUTER JOIN [BookingDB].dbo.Luggage l ON li.LuggageID = l.LuggageID
			            LEFT OUTER JOIN [BookingDB].dbo.LuggageOperations lo ON l.LuggageID = lo.LuggageID
	            WHERE
		            lo.DeskID = cte.DeskID
		            AND lf.DateCreated > cte.OperationDateTime
		            AND lf.IsCancelled = 1
	            UNION ALL
	            SELECT
		            -SUM( Amount ) Cash
	            FROM [BookingDB].dbo.DeskCollections
	            WHERE DeskID = cte.DeskID AND
		            DateCreated > cte.OperationDateTime
            ) q
            WHERE cte.rn = 1
            GROUP BY cte.DeskID, cte.Description, cte.Balance";

            Sql(script);
        }

        public override void Down()
        {
            Sql("DROP VIEW dbo.VDeskBalances");
        }
    }
}
