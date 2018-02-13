namespace LightMethods.Survey.Models.Migrations
{
    using LightMethods.Survey.Models.DAL;
    using LightMethods.Survey.Models.DTO;
    using LightMethods.Survey.Models.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using LightMethods.Survey.Models.Services.Identity;

    public sealed class Configuration : DbMigrationsConfiguration<LightMethods.Survey.Models.DAL.SurveyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LightMethods.Survey.Models.DAL.SurveyContext";
        }

        protected override void Seed(SurveyContext context)
        {
            base.Seed(context);
            AddBasicData(context);
            AddSampleData(context);
        }

        public void AddBasicData(SurveyContext context)
        {

            var userManager = new ApplicationUserManager(new ApplicationUserStore(context));
            var roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));

            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("cadbeb33-4c7a-428c-9520-6756a00c8697"), Name = Role.SYSTEM_ADMINSTRATOR });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("6d923482-7907-422a-b054-1b58b1bbbc81"), Name = Role.PLATFORM_ADMINISTRATOR });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("c452e933-dc83-44f2-a3ab-59cf2eb7b0a2"), Name = Role.ORG_ADMINSTRATOR });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("09898aea-0925-4472-9124-11026a17302a"), Name = Role.ORG_TEAM_USER });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("f7bb6773-d74a-42aa-8686-333f5a753d24"), Name = Role.ORG_TEAM_MANAGER });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("1ee7f025-0c19-4cb8-a7c9-e24497833df3"), Name = Role.ORG_USER });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("453a3eee-2de3-47d5-b041-3ff01ea13cd9"), Name = Role.ORG_USER_MANAGMENT });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("9aaa8582-12fd-45d2-ac19-65d5f1deabfc"), Name = Role.ORG_PROJECT_MANAGMENT });
            roleManager.AddOrUpdateRole(new Role { Id = Guid.Parse("52e030ef-e3d4-4976-b3d7-a2353978b099"), Name = Role.ORG_TEMPLATES_MANAGMENT });

            var adminEmail = "khajoo@gmail.com";
            var superUser = new SuperUser() { Id = Guid.Parse("427eecf5-8ed4-4a6f-820d-7901acedf3bb"), UserName = adminEmail, Email = adminEmail };

            userManager.AddOrUpdateUser(superUser, "Test1234");
            userManager.AddToRole(superUser.Id, Role.SYSTEM_ADMINSTRATOR);

            context.AddressTypes.AddOrUpdate(new AddressType() { Id = Guid.Parse("2d006d8c-8543-42c7-a506-6b1a6570a7e1"), Name = "Home" });
            context.AddressTypes.AddOrUpdate(new AddressType() { Id = Guid.Parse("5835af54-9f7e-4c59-bfe0-f990809e87d1"), Name = "Work" });
            context.SaveChanges();

            context.ContactNumberTypes.AddOrUpdate(new ContactNumberType() { Id = Guid.Parse("1cbe32a5-7398-4c23-b8df-91ff48c0c89d"), Name = "Home" });
            context.ContactNumberTypes.AddOrUpdate(new ContactNumberType() { Id = Guid.Parse("4380daa1-7b94-463b-b544-1de512f3afe7"), Name = "Work" });
            context.ContactNumberTypes.AddOrUpdate(new ContactNumberType() { Id = Guid.Parse("9eab2c57-e585-4505-a481-eb9c0bb1bfc5"), Name = "Mobile" });
            context.SaveChanges();

            context.OrgUserTypes.AddOrUpdate(new OrgUserType() { Id = Guid.Parse("22d3271a-0e84-440a-8bd1-644b2f10e9fb"), Order = 10, SystemName = "Administrator", Name = "Administrator" });
            context.OrgUserTypes.AddOrUpdate(new OrgUserType() { Id = Guid.Parse("e8f53f9f-05e0-4d60-bb58-f9fe6f12b0a4"), Order = 20, SystemName = "Manager", Name = "Manager" });
            context.OrgUserTypes.AddOrUpdate(new OrgUserType() { Id = Guid.Parse("379c989a-9919-4338-a468-a7c20eb76e28"), Order = 30, SystemName = "TeamUser", Name = "Team user" });
            context.OrgUserTypes.AddOrUpdate(new OrgUserType() { Id = Guid.Parse("5c87861a-abdf-4d96-90b2-338ca761700b"), Order = 40, SystemName = "ExternalUser", Name = "External user" });
            context.SaveChanges();

            context.SeverityLevels.AddOrUpdate(new SeverityLevel() { Id = Guid.Parse("a4c21c55-efbf-4d48-a38a-1eec40bc9350"), Order = 10, SystemName = "Minimal", Name = "Minimal" });
            context.SeverityLevels.AddOrUpdate(new SeverityLevel() { Id = Guid.Parse("70171c68-4e3c-4e40-ae4d-a88447756082"), Order = 20, SystemName = "Medium", Name = "Medium" });
            context.SeverityLevels.AddOrUpdate(new SeverityLevel() { Id = Guid.Parse("7636b628-c520-4322-a0ac-2d467839fc89"), Order = 30, SystemName = "Critical", Name = "Critical" });
            context.SaveChanges();

            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("4825ad24-7154-4bfc-b64c-8717560b7e69"), Order = 10, SystemName = "Line", Name = "Line" });
            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("0d169039-1297-41b8-b9a3-9bc8208d9944"), Order = 20, SystemName = "Column", Name = "Column" });
            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("094b3eb0-c35b-4adb-ad42-3c44cfc4902d"), Order = 30, SystemName = "Bar", Name = "Bar" });
            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("b1edfba9-da27-4e56-b099-55480948b796"), Order = 40, SystemName = "Spline", Name = "Spline" });
            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("d01d3ee8-d495-449a-bb2b-eca2c5f53ff8"), Order = 50, SystemName = "Scatter ", Name = "Scatter " });
            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("49693931-94c0-4eb2-ad4b-ec6e95435cf1"), Order = 60, SystemName = "Area", Name = "Area" });
            context.ChartSerieTypes.AddOrUpdate(new ChartSerieType() { Id = Guid.Parse("ae838bf8-6667-441f-ba0c-189fcdbe8b49"), Order = 70, SystemName = "Areaspline", Name = "Areaspline" });
            context.SaveChanges();

            context.AdultTitles.AddOrUpdate(new AdultTitle() { Id = Guid.Parse("e90cb5ae-bb34-4243-ad8d-6ff342d93724"), Order = 10, SystemName = "Dr", Name = "Dr" });
            context.AdultTitles.AddOrUpdate(new AdultTitle() { Id = Guid.Parse("cb2b1034-2d7c-42fa-9756-ca7179d7ef7e"), Order = 20, SystemName = "Mrs", Name = "Mrs" });
            context.AdultTitles.AddOrUpdate(new AdultTitle() { Id = Guid.Parse("9bc07951-45fd-4530-89b0-cd830eb6ff2d"), Order = 30, SystemName = "Mr", Name = "Mr" });
            context.AdultTitles.AddOrUpdate(new AdultTitle() { Id = Guid.Parse("95a9763c-3bd8-4d94-b23f-9d5f51ef1f55"), Order = 40, SystemName = "Miss", Name = "Miss" });
            context.SaveChanges();

            context.ReportListDataTypes.AddOrUpdate(new ReportListDataType() { Id = Guid.Parse("18194a41-f23d-43ed-a298-8fc6a03c3a91"), Order = 10, SystemName = "Project", Name = "Project" });
            context.ReportListDataTypes.AddOrUpdate(new ReportListDataType() { Id = Guid.Parse("1c342f84-6b92-4dd4-8d28-554d0a9cce9b"), Order = 40, SystemName = "Commentaries", Name = "Commentaries" });
            context.SaveChanges();

            context.Languages.AddOrUpdate(new Language() { Id = Guid.Parse("aa43288c-7da3-4bce-8086-3b8f0eb429c3"), Order = 10, SystemName = "English", Name = "English", Calture = "en-GB" });
            context.Languages.AddOrUpdate(new Language() { Id = Guid.Parse("27f78e81-58de-473f-9609-357238d76445"), Order = 20, SystemName = "Persian", Name = "Persian", Calture = "fa-IR" });
            context.SaveChanges();

            context.Calendars.AddOrUpdate(new Calendar() { Id = Guid.Parse("36168adc-2462-4ad9-b1e0-9c37d4388143"), Order = 10, SystemName = "Gregorian", Name = "Gregorian" });
            context.Calendars.AddOrUpdate(new Calendar() { Id = Guid.Parse("bf51856c-616c-4289-84e7-84ef9d282a4f"), Order = 20, SystemName = "Persian", Name = "Persian" });
            context.SaveChanges();

            context.AttachmentTypes.AddOrUpdate(new AttachmentType() { Id = Guid.Parse("53d7285b-0f7e-434e-9319-ca1b12c1344c"), Name = "Document", MaxFileSize = 1024, AllowedExtensions = "doc,docx,txt,pdf,rtf,csv,xls,xlsx" });
            context.AttachmentTypes.AddOrUpdate(new AttachmentType() { Id = Guid.Parse("9252a389-459b-48c9-81b8-475a6a35a706"), Name = "Image", MaxFileSize = 2048, AllowedExtensions = "jpg,jpeg,png,gif" });
            context.AttachmentTypes.AddOrUpdate(new AttachmentType() { Id = Guid.Parse("d87266db-6fd1-4f9b-8afa-28b1af6bb5c6"), Name = "Audio", MaxFileSize = 2048, AllowedExtensions = "wav,mp3,wma,amr,aac,ogg" });
            context.AttachmentTypes.AddOrUpdate(new AttachmentType() { Id = Guid.Parse("8022c0d5-ecc7-4ad0-be45-3cfc6c12f0a3"), Name = "Video", MaxFileSize = 8192, AllowedExtensions = "avi,mpg,mpeg,mp4,mov,wmv" });
            context.SaveChanges();


            context.Settings.AddOrUpdate(new Settings());
            context.SaveChanges();

        }

        private void AddSampleData(SurveyContext context)
        {
            var userManager = new ApplicationUserManager(new ApplicationUserStore(context));
            var roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));

            try
            {
                Project CurrentProject = null;

                var org = new Organisation()
                {
                    Id = Guid.Parse("55ce7ae6-9378-4a68-8d05-c703556c69d8"),
                    Name = "First test organisation",
                    TelNumber = "0209390499",
                    AddressLine1 = "110 Kings road",
                    AddressLine2 = "Regent street",
                    Town = "London",
                    County = "London",
                    Postcode = "EC23 4AD",
                    DefaultLanguageId = LanguagesRepository.English.Id,
                    DefaultCalendarId = CalendarsRepository.Gregorian.Id,
                    IsActive = true,
                    SubscriptionEnabled = true,
                    SubscriptionMonthlyRate = 6M
                };

                context.Organisations.AddOrUpdate(org);
                var user = new OrgUser() { Id = Guid.Parse("b3c19356-d11d-48f2-a3a8-69392a7b4e7b"), OrganisationId = org.Id, IsRootUser = true, Email = "admin@test.t", UserName = "admin@test.t", TypeId = OrgUserTypesRepository.Administrator.Id, LastLogin = new DateTime(2015, 1, 1), IsWebUser = true, IsMobileUser = true };
                userManager.AddOrUpdateUser(user, "Test1234");

                OrgUserTypesRepository.Administrator.GetRoles().ToList().ForEach(role => userManager.AddToRole(user.Id, role));

                context.SaveChanges();
                org.RootUser = user;
                context.SaveChanges();



                CurrentProject = new Project() { Id = Guid.Parse("cb7f09a2-1823-4f60-820e-3fedc462fe76"), Number = "123", Name = "Test Project 1", StartDate = new DateTime(2015, 1, 1), Flagged = true, OrganisationId = org.Id };
                context.Projects.AddOrUpdate(CurrentProject);
                context.SaveChanges();

                var dataList = new DataList() { Id = Guid.Parse("884505e1-97c8-4602-8c00-f75ae08d99ab"), OrganisationId = org.Id, Name = "Colors" };
                context.DataLists.AddOrUpdate(dataList);

                context.DataListItems.AddOrUpdate(new DataListItem { Id = Guid.Parse("0a138f19-2e98-4030-b9e9-78527043c1c2"), DataListId = dataList.Id, Text = "Black", Value = 0, Order = 1 });
                context.DataListItems.AddOrUpdate(new DataListItem { Id = Guid.Parse("32d75f9b-7da6-43b5-8f3f-b7830dc6e5f7"), DataListId = dataList.Id, Text = "White", Value = 1, Order = 2 });
                context.DataListItems.AddOrUpdate(new DataListItem { Id = Guid.Parse("298cdd84-4cd1-481f-875e-c2c9adce43bc"), DataListId = dataList.Id, Text = "Blue", Value = 2, Order = 3 });
                context.DataListItems.AddOrUpdate(new DataListItem { Id = Guid.Parse("8e63ad2d-6872-4e6d-86e6-55782450f991"), DataListId = dataList.Id, Text = "Red", Value = 3, Order = 4 });

                context.SaveChanges();

                var cat = new FormTemplateCategory() { Id = Guid.Parse("5e7d5a6f-6838-4e09-bcee-e07778c26f44"), Title = "First category", OrganisationId = org.Id };
                context.FormTemplateCategories.AddOrUpdate(cat);
                context.SaveChanges();

                var template = new FormTemplate() { Id = Guid.Parse("52692cf5-fd17-4fc6-b72b-b65b7e8d4e98"), ProjectId = CurrentProject.Id, Code = "101", Title = "First Form", Description = "This is the first from.", Colour = "#ddff00", Version = 1.0, FormTemplateCategoryId = cat.Id, IsPublished = true, OrganisationId = org.Id, CreatedById = user.Id };
                context.FormTemplates.AddOrUpdate(template);
                context.SaveChanges();

                var category1 = new MetricGroup() { Id = Guid.Parse("bedb627d-155d-4807-93dd-40cbb1fa335d"), Title = "First questions", FormTemplateId = template.Id, Order = template.GetMaxGroupOrder() };
                template.AddGroup(category1);
                context.MetricGroups.AddOrUpdate(category1);

                var category2 = new MetricGroup() { Id = Guid.Parse("45ca1db0-820b-4271-8f26-7a75e7e73c80"), Title = "Second questions", FormTemplateId = template.Id, Order = template.GetMaxGroupOrder() };
                template.AddGroup(category2);
                context.MetricGroups.AddOrUpdate(category2);

                var category3 = new MetricGroup() { Id = Guid.Parse("0b76ee76-405a-434a-9bca-4e5b3f1eb5e7"), Title = "Third questions", FormTemplateId = template.Id, Order = template.GetMaxGroupOrder() };
                template.AddGroup(category3);
                context.MetricGroups.AddOrUpdate(category3);
                context.SaveChanges();

                var metric1 = new FreeTextMetric() { Id = Guid.Parse("f6e5b494-d845-48dc-a783-e3e93b340b3a"), ShortTitle = "q1", Description = "What is the answer1", NumberOfLine = 1, MaxLength = 10, Order = category1.GetMaxMetricOrder(), Mandatory = true };
                category1.AddMetric(metric1);
                context.FreeTextMetrics.AddOrUpdate(metric1);

                var metric2 = new FreeTextMetric() { Id = Guid.Parse("8a85e16c-9abb-4f99-83fc-564605bbd52f"), ShortTitle = "q2", Description = "What is the answer2", NumberOfLine = 3, Order = category1.GetMaxMetricOrder(), Mandatory = true };
                category1.AddMetric(metric2);
                context.FreeTextMetrics.AddOrUpdate(metric2);

                var metric3 = new FreeTextMetric() { Id = Guid.Parse("b2f747bc-b403-47de-95c2-2efd8e1ebeb6"), ShortTitle = "q3", Description = "What is the answer3", NumberOfLine = 3, Order = category2.GetMaxMetricOrder() };
                category2.AddMetric(metric3);
                context.FreeTextMetrics.AddOrUpdate(metric3);

                var metric4 = new FreeTextMetric() { Id = Guid.Parse("a8373528-0d92-4d91-bff0-5397700644c8"), ShortTitle = "q4", Description = "What is the answer4", NumberOfLine = 2, Order = category2.GetMaxMetricOrder() };
                category2.AddMetric(metric4);
                context.FreeTextMetrics.AddOrUpdate(metric4);

                var metric5 = new FreeTextMetric() { Id = Guid.Parse("14531856-8b05-4711-850e-d0ac00173732"), ShortTitle = "q5", Description = "What is the answer5", NumberOfLine = 2, Order = category3.GetMaxMetricOrder() };
                category3.AddMetric(metric5);
                context.FreeTextMetrics.AddOrUpdate(metric5);

                var metric6 = new RateMetric() { Id = Guid.Parse("ae9399eb-80b2-4944-be22-55b561f10bc5"), ShortTitle = "r1", Description = "What is the rate1", MinValue = 1, MaxValue = 5, DefaultValue = 1, Order = category1.GetMaxMetricOrder() };
                category1.AddMetric(metric6);
                context.RateMetrics.AddOrUpdate(metric6);

                var metric7 = new DateMetric() { Id = Guid.Parse("68ce9679-14b5-4e00-97d6-38ad6db54f54"), ShortTitle = "d1", Description = "Date of Event", Order = category1.GetMaxMetricOrder(), HasTimeValue = false };
                category1.AddMetric(metric7);
                context.DateMetrics.AddOrUpdate(metric7);

                var metric8 = new DichotomousMetric() { Id = Guid.Parse("e254bf53-2ddb-4817-a9d7-6d8513696a2f"), ShortTitle = "yn1", Description = "Are you ok?", Order = category1.GetMaxMetricOrder() };
                category1.AddMetric(metric8);
                context.DichotomousMetrics.AddOrUpdate(metric8);

                context.SaveChanges();

                //var data = new FilledForm() { Id = Guid.Parse("390f8abc-dc99-411a-a22f-8cf43313337a"), FormTemplateId = template.Id, ProjectId = CurrentProject.Id, FilledById = user.Id };
                //context.FilledForms.AddOrUpdate(data);

                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("24bacf38-52ee-40e5-916c-a3f81a84eea5"), FilledFormId = data.Id, MetricId = metric1.Id, TextValue = "answer 1" });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("cdb38534-f412-4397-aa1e-51a587db402f"), FilledFormId = data.Id, MetricId = metric2.Id, TextValue = "answer 2" });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("b3d3dc58-5467-42ac-8d2a-3e327635a72d"), FilledFormId = data.Id, MetricId = metric3.Id, TextValue = "answer 3" });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("d3efb64f-8bf1-4916-9dd0-07fc11ec171e"), FilledFormId = data.Id, MetricId = metric4.Id, TextValue = "answer 4" });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("9a0e941e-f28a-4bf3-9a23-edeecf4368df"), FilledFormId = data.Id, MetricId = metric5.Id, TextValue = "answer 5" });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("a00e12d0-92a5-4737-9dd3-b83f8a71e320"), FilledFormId = data.Id, MetricId = metric6.Id, NumericValue = 3 });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("b2f46bdc-f987-4a2d-bcc7-bb7c4f592f21"), FilledFormId = data.Id, MetricId = metric7.Id, DateValue = DateTime.Now });
                //context.FormValues.AddOrUpdate(new FormValue() { Id = Guid.Parse("747a6bbf-c439-40e4-a76e-d03b68ed1614"), FilledFormId = data.Id, MetricId = metric8.Id, BoolValue = true });

                //context.SaveChanges();

                var reportCat = new ReportTemplateCategory() { Id = Guid.Parse("00b2ea95-dcaf-480a-b470-6d012a5f1d6e"), OrganisationId = org.Id, Title = "First report category" };
                context.ReportTemplateCategories.AddOrUpdate(reportCat);

                var reportTemplate = new ReportTemplate() { Id = Guid.Parse("04d3727c-738a-44b7-952d-7c74e2383c4b"), OrganisationId = org.Id, Name = "First sample report", CategoryId = reportCat.Id, Description = "", IsPublished = true };
                context.ReportTemplates.AddOrUpdate(reportTemplate);

                context.SaveChanges();


            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
        }

    }
}
