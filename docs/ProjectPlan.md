# TrailKit Project Plan

Keep this file current. The professor reviews it in the repository.

## Use case owners

| Member | Use cases |
| --- | --- |
| Alison | UC2 Manage Gear Catalog, UC6 View Business Insights |
| Caleb | UC3 Pick Up and Return Gear, UC5 Track Maintenance |
| Layan | UC1 Reserve Gear, UC7 Earn and Redeem Loyalty Points |
| Utsah | UC4 Manage Customers, UC8 Create Account |

## Class ownership

| Member | Classes |
| --- | --- |
| Alison | Gear, Tent, Kayak, Bike, CatalogService, GearValidator, GearRepository, DbSeeder, CatalogController, GearApiController, InsightsService, ProfitCalculator, InsightsRepository, InsightsController, InsightsApiController, CatalogServiceTests, ProfitCalculatorTests |
| Caleb | Rental, DepositHold, MaintenanceRecord, RentalService, RentalChargeCalculator, IChargePolicy, StandardChargePolicy, MemberChargePolicy, MaintenanceService, MaintenanceMonitor, RentalRepository, MaintenanceRepository, CounterController, RentalsApiController, MaintenanceController, MaintenanceApiController, RentalChargeCalculatorTests, MaintenanceMonitorTests |
| Layan | Reservation, ReservationStatus, LoyaltyAccount, LoyaltyTier, ReservationService, AvailabilityChecker, LoyaltyService, PointsCalculator, IRepository, Repository, ReservationRepository, LoyaltyRepository, ReservationsController, ReservationsApiController, LoyaltyController, LoyaltyApiController, ReservationServiceTests, AvailabilityCheckerTests, PointsCalculatorTests |
| Utsah | Customer, StandardCustomer, MemberCustomer, Charge, UserAccount, CustomerService, StandingEvaluator, AccountService, RegistrationValidator, TrailKitDbContext, CustomerRepository, AccountRepository, CustomersController, HomeController, AccountController, AccountApiController, StandingEvaluatorTests, RegistrationValidatorTests |

## Milestones

| Phase | Work | Status |
| --- | --- | --- |
| 1. Setup | Solution, projects, repository shared with professor | Done |
| 2. Foundation | Entities, TrailKitDbContext, Repository base, first migration, seed data, login and roles | Not started |
| 3. First use cases | UC1 to UC4 end to end, with tests | Not started |
| 4. Second use cases | UC5 to UC8 end to end, with tests | Not started |
| 5. IIS test | Run the full app on IIS with SQL Server | Not started |
| 6. Final | Updated proposal, UML model, presentation | Not started |

## Open decisions

- Maintenance thresholds per gear type (proposed: tents 15, kayaks 10, bikes 8 rentals)
- Loyalty point rates and tiers (proposed: 1 point per dollar, 100 points = $5, Silver 500, Gold 1,500)
- Suspension rule (proposed: over $50 unpaid or 2 late returns in 90 days)
- Business insights cost flag (proposed: maintenance over 40% of revenue)

## Changes from the proposal

Record every change here as it happens, for the final report.

| Date | Change | Reason |
| --- | --- | --- |
| | | |
