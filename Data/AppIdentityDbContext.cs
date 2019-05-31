using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvibaWeb.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionOperation> CollectionOperations { get; set; }
        
        public DbSet<Desk> Desks { get; set; }
        public DbSet<DeskGroup> DeskGroups { get; set; }
        public DbSet<CollectionOperationType> CollectionOperationTypes { get; set; }

        public DbSet<Expenditure> Expenditures { get; set; }
        public DbSet<ExpenditureObject> ExpenditureObjects { get; set; }
        public DbSet<ExpenditureType> ExpenditureTypes { get; set; }
        public DbSet<ExpenditureOperation> ExpenditureOperations { get; set; }

        public DbSet<AcceptedCollector> AcceptedCollectors { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<FinancialAccount> FinancialAccounts { get; set; }
        public DbSet<FinancialAccountOperation> FinancialAccountOperations { get; set; }

        public DbSet<Counterparty> Counterparties { get; set; }
        public DbSet<CounterpartyType> CounterpartyTypes { get; set; }

        public DbSet<SubagentDesk> SubagentDesks { get; set; }

        public DbSet<TransitAccount> TransitAccounts { get; set; }
        public DbSet<TransitAccountDebit> TransitAccountDebits { get; set; }
        public DbSet<TransitAccountCredit> TransitAccountCredits { get; set; }
        public DbSet<TransitAccountCreditOperation> TransitAccountCreditOperations { get; set; }

        public DbSet<LoanGroup> LoanGroups { get; set; }

        public DbSet<OfficeDebit> OfficeDebits { get; set; }
        public DbSet<OfficeDebitOperation> OfficeDebitOperations { get; set; }

        public DbSet<VDeskBalance> VDeskBalances { get; set; }
        public DbSet<VCorpBalance> VCorpBalances { get; set; }

        public DbSet<SettingsValue> SettingsValues { get; set; }

        public DbSet<UserCheckIn> UserCheckIns { get; set; }

        public DbSet<KRSCancelRequest> KRSCancelRequests { get; set; }
        public DbSet<KRSCancelRequestOperation> KRSCancelRequestOperations { get; set; }

        public DbSet<VKRSCancelRequest> VKRSCancelRequests { get; set; }

        public DbSet<CorporatorReceipt> CorporatorReceipts { get; set; }
        public DbSet<CorporatorReceiptItem> CorporatorReceiptItems { get; set; }
        public DbSet<CorporatorReceiptOperation> CorporatorReceiptOperations { get; set; }

        public DbSet<VTicketOperation> VTicketOperations { get; set; }
        public DbSet<VReceiptTicketInfo> VReceiptTicketInfo { get; set; }
        public DbSet<VReceiptLuggageInfo> VReceiptLuggageInfo { get; set; }

        public DbSet<VBookingCorpReceiptInfo> VBookingCorpReceiptInfo { get; set; }

        public DbSet<CorporatorReceiptMultiPayment> CorporatorReceiptMultiPayments { get; set; }

        public DbSet<VBookingCorporator> VBookingCorporators { get; set; }

        public DbSet<CorporatorFeeRate> CorporatorFeeRates { get; set; }

        public DbSet<CorporatorDocument> CorporatorDocuments { get; set; }

        public DbSet<CorporatorAccount> CorporatorAccounts { get; set; }

        public DbSet<TicketCancelOperation> TicketCancelOperations { get; set; }

        public DbSet<VTicketCancelList> VTicketCancelList { get; set; }

        public DbSet<VTicketPDFInfo> VTicketPDFInfo { get; set; }

        public DbSet<VTicketSegmentPDFInfo> VTicketSegmentPDFInfo { get; set; }

        public DbSet<VTicketTaxPDFInfo> VTicketTaxPDFInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Query<BookingOperator>();
            modelBuilder.Query<Corporator>();

            modelBuilder.Entity<AcceptedCollector>()
                .HasKey(u => new { u.ProviderId, u.CollectorId });

            modelBuilder.Entity<AcceptedCollector>()
                .HasOne(e => e.Provider)
                .WithMany(e => e.AcceptedCollectors)
                .HasForeignKey(e => e.ProviderId);

            modelBuilder.Entity<AppUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}