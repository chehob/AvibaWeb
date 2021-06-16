using System.Data.SqlClient;
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

        public DbSet<Income> Incomes { get; set; }
        public DbSet<IncomeOperation> IncomeOperations { get; set; }

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
        public DbSet<CorporatorDocumentFeeItem> CorporatorDocumentFeeItems { get; set; }

        public DbSet<CorporatorAccount> CorporatorAccounts { get; set; }

        public DbSet<TicketCancelOperation> TicketCancelOperations { get; set; }

        public DbSet<VTicketCancelList> VTicketCancelList { get; set; }

        public DbSet<VTicketPDFInfo> VTicketPDFInfo { get; set; }

        public DbSet<VTicketSegmentPDFInfo> VTicketSegmentPDFInfo { get; set; }

        public DbSet<VTicketTaxPDFInfo> VTicketTaxPDFInfo { get; set; }

        public DbSet<ProviderBalanceTransaction> ProviderBalanceTransactions { get; set; }
        public DbSet<OfficeBalanceHistory> OfficeBalanceHistory { get; set; }

        public DbSet<CorporatorAccountTransaction> CorporatorAccountTransactions { get; set; }

        public DbSet<ProviderAgentFeeTransaction> ProviderAgentFeeTransactions { get; set; }

        public DbSet<IncomingExpenditure> IncomingExpenditures { get; set; }

        public DbSet<_1CUpload> _1CUploads { get; set; }

        public DbSet<_1CUploadData> _1CUploadData { get; set; }

        public DbSet<_1СProviderDocument> _1СProviderDocuments { get; set; }

        public DbSet<VIncomeKRS> VIncomeKRS { get; set; }

        public DbSet<ServiceReceipt> ServiceReceipts { get; set; }
        public DbSet<ServiceReceiptItem> ServiceReceiptItems { get; set; }
        public DbSet<ServiceReceiptOperation> ServiceReceiptOperations { get; set; }

        public DbSet<VServiceReceiptIncomeInfo> VServiceReceiptIncomeInfo { get; set; }

        public DbSet<SubagentFeeTransaction> SubagentFeeTransactions { get; set; }

        public DbSet<BMDeskGroup> BMDeskGroups { get; set; }

        public DbSet<VSessionTypes> VSessionTypes { get; set; }
        public DbSet<VCities> VCities { get; set; }
        public DbSet<VAirlines> VAirlines { get; set; }

        public DbSet<VBookingManagementSales> VBookingManagementSales { get; set; }
        public DbSet<VBookingManagementLuggage> VBookingManagementLuggage { get; set; }
        public DbSet<VBookingManagementOperation> VBookingManagementOperations { get; set; }

        public DbSet<PKReceiptRule> PKReceiptRules { get; set; }

        public DbSet<VBookingManagementPaycheck> VBookingManagementPaycheck { get; set; }

        public DbSet<S7ProviderBalance> S7ProviderBalance { get; set; }

        public DbSet<LoanExpenditure> LoanExpenditures { get; set; }
        public DbSet<LoanExpenditureOperation> LoanExpenditureOperations { get; set; }

        public DbSet<VCustomIncomeInfo> VCustomIncomeInfo { get; set; }

        public DbSet<CorporatorReceiptTemplate> CorporatorReceiptTemplates { get; set; }

        public DbSet<VLogData> VLogData { get; set; }

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

        public virtual void SetUserContext(string userId)
        {
            var idParam = new SqlParameter("@userId", userId);
            Database.ExecuteSqlCommand("SetUserContext @userId", idParam);
        }
    }
}