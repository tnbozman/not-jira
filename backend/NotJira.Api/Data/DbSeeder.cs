using NotJira.Api.Models;

namespace NotJira.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Only seed if there are no projects yet
        if (context.Projects.Any())
            return;

        var now = new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc);

        // ── Project ─────────────────────────────────────────────────
        var project = new Project
        {
            Key = "PETPAL",
            Name = "PetPal – Pet Adoption Platform",
            Description = "An online platform connecting animal shelters with potential pet adopters, streamlining the adoption process and improving post-adoption support.",
            CreatedAt = now.AddDays(-60),
            UpdatedAt = now
        };
        context.Projects.Add(project);
        context.SaveChanges();

        // ── Project Members ─────────────────────────────────────────
        var members = new List<ProjectMember>
        {
            new() { ProjectId = project.Id, UserId = "user-001", UserName = "Alice Rivera", UserEmail = "alice@petpal.dev", Role = ProjectRole.ProductManager, AddedAt = now.AddDays(-60) },
            new() { ProjectId = project.Id, UserId = "user-002", UserName = "Bob Chen", UserEmail = "bob@petpal.dev", Role = ProjectRole.Developer, AddedAt = now.AddDays(-58) },
            new() { ProjectId = project.Id, UserId = "user-003", UserName = "Carol Nguyen", UserEmail = "carol@petpal.dev", Role = ProjectRole.Developer, AddedAt = now.AddDays(-58) },
            new() { ProjectId = project.Id, UserId = "user-004", UserName = "Dave Okafor", UserEmail = "dave@petpal.dev", Role = ProjectRole.ProjectSponsor, AddedAt = now.AddDays(-60) }
        };
        context.ProjectMembers.AddRange(members);
        context.SaveChanges();

        // ── Tags ────────────────────────────────────────────────────
        var tagUx = new Tag { Name = "UX", Description = "User experience related", ProjectId = project.Id, CreatedAt = now.AddDays(-55) };
        var tagShelter = new Tag { Name = "Shelter", Description = "Shelter operations", ProjectId = project.Id, CreatedAt = now.AddDays(-55) };
        var tagAdopter = new Tag { Name = "Adopter", Description = "Adopter experience", ProjectId = project.Id, CreatedAt = now.AddDays(-55) };
        var tagCompliance = new Tag { Name = "Compliance", Description = "Legal & regulatory", ProjectId = project.Id, CreatedAt = now.AddDays(-55) };
        var tagIntegration = new Tag { Name = "Integration", Description = "Third-party integration", ProjectId = project.Id, CreatedAt = now.AddDays(-55) };
        context.Tags.AddRange(tagUx, tagShelter, tagAdopter, tagCompliance, tagIntegration);
        context.SaveChanges();

        // ── External Entities (stakeholders) ────────────────────────
        var shelterDirector = new ExternalEntity
        {
            Type = EntityType.Person,
            Name = "Maria Lopez",
            Email = "maria@happypawsshelter.org",
            Organization = "Happy Paws Shelter",
            Phone = "+1-555-0101",
            Notes = "Key decision-maker for shelter workflow. Very vocal about reducing paperwork.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-50),
            UpdatedAt = now.AddDays(-10)
        };
        var vetClinic = new ExternalEntity
        {
            Type = EntityType.Client,
            Name = "Greenfield Veterinary Clinic",
            Email = "info@greenfieldvet.com",
            Organization = "Greenfield Veterinary Clinic",
            Phone = "+1-555-0202",
            Notes = "Partner clinic – provides health records for adoption listings.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-48),
            UpdatedAt = now.AddDays(-15)
        };
        var adopter = new ExternalEntity
        {
            Type = EntityType.Person,
            Name = "James Park",
            Email = "jpark@gmail.com",
            Organization = null,
            Phone = "+1-555-0303",
            Notes = "Adopted two dogs last year. Interested in fostering features.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-45),
            UpdatedAt = now.AddDays(-12)
        };
        var animalControl = new ExternalEntity
        {
            Type = EntityType.Client,
            Name = "Metro Animal Control",
            Email = "contact@metroanimalcontrol.gov",
            Organization = "Metro Animal Control",
            Phone = "+1-555-0404",
            Notes = "Government partner – requires compliance reporting.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-44),
            UpdatedAt = now.AddDays(-20)
        };
        context.ExternalEntities.AddRange(shelterDirector, vetClinic, adopter, animalControl);
        context.SaveChanges();

        // ── Entity Tags ─────────────────────────────────────────────
        context.Set<EntityTag>().AddRange(
            new EntityTag { ExternalEntityId = shelterDirector.Id, TagId = tagShelter.Id },
            new EntityTag { ExternalEntityId = shelterDirector.Id, TagId = tagUx.Id },
            new EntityTag { ExternalEntityId = vetClinic.Id, TagId = tagIntegration.Id },
            new EntityTag { ExternalEntityId = adopter.Id, TagId = tagAdopter.Id },
            new EntityTag { ExternalEntityId = animalControl.Id, TagId = tagCompliance.Id }
        );
        context.SaveChanges();

        // ── Problems ────────────────────────────────────────────────
        var prob1 = new Problem
        {
            Description = "Shelter staff spend 3+ hours daily on manual data entry for each animal listing.",
            Severity = Severity.Critical,
            Context = "Paper forms are still used for intake. No integration with existing shelter management systems.",
            ExternalEntityId = shelterDirector.Id,
            CreatedAt = now.AddDays(-45),
            UpdatedAt = now.AddDays(-10)
        };
        var prob2 = new Problem
        {
            Description = "Adopters have no way to verify an animal's vaccination and health history before visiting.",
            Severity = Severity.High,
            Context = "Vet records are kept in separate systems; shelters manually photocopy them.",
            ExternalEntityId = vetClinic.Id,
            CreatedAt = now.AddDays(-44),
            UpdatedAt = now.AddDays(-15)
        };
        var prob3 = new Problem
        {
            Description = "Post-adoption follow-up is entirely ad-hoc, leading to poor outcomes tracking.",
            Severity = Severity.Medium,
            Context = "Shelters lose track of animals after adoption. No structured check-in process.",
            ExternalEntityId = adopter.Id,
            CreatedAt = now.AddDays(-42),
            UpdatedAt = now.AddDays(-12)
        };
        var prob4 = new Problem
        {
            Description = "Compliance reports for animal control take weeks to compile manually.",
            Severity = Severity.High,
            Context = "Government requires quarterly reporting on adoption rates, returns, and euthanasia numbers.",
            ExternalEntityId = animalControl.Id,
            CreatedAt = now.AddDays(-40),
            UpdatedAt = now.AddDays(-20)
        };
        var prob5 = new Problem
        {
            Description = "Search filters on existing adoption sites are too basic – adopters can't find compatible pets.",
            Severity = Severity.Medium,
            Context = "Current sites only filter by species and breed, not temperament, energy level, or living situation compatibility.",
            ExternalEntityId = adopter.Id,
            CreatedAt = now.AddDays(-38),
            UpdatedAt = now.AddDays(-12)
        };
        context.Problems.AddRange(prob1, prob2, prob3, prob4, prob5);
        context.SaveChanges();

        // ── Problem Tags ────────────────────────────────────────────
        context.Set<ProblemTag>().AddRange(
            new ProblemTag { ProblemId = prob1.Id, TagId = tagShelter.Id },
            new ProblemTag { ProblemId = prob2.Id, TagId = tagIntegration.Id },
            new ProblemTag { ProblemId = prob3.Id, TagId = tagAdopter.Id },
            new ProblemTag { ProblemId = prob4.Id, TagId = tagCompliance.Id },
            new ProblemTag { ProblemId = prob5.Id, TagId = tagUx.Id }
        );
        context.SaveChanges();

        // ── Outcomes ────────────────────────────────────────────────
        var out1 = new Outcome
        {
            Description = "Reduce shelter data-entry time by 80% through automated animal intake.",
            Priority = Priority.Critical,
            Context = "Directly addresses shelter staff burnout and operational cost.",
            ExternalEntityId = shelterDirector.Id,
            CreatedAt = now.AddDays(-40),
            UpdatedAt = now.AddDays(-10)
        };
        var out2 = new Outcome
        {
            Description = "Provide adopters with verified, real-time health records for every listed animal.",
            Priority = Priority.High,
            Context = "Builds trust and reduces shelter visit no-shows.",
            ExternalEntityId = vetClinic.Id,
            CreatedAt = now.AddDays(-39),
            UpdatedAt = now.AddDays(-15)
        };
        var out3 = new Outcome
        {
            Description = "Achieve 90% post-adoption check-in completion rate within 30 days.",
            Priority = Priority.Medium,
            Context = "Improves animal welfare tracking and adopter satisfaction.",
            ExternalEntityId = adopter.Id,
            CreatedAt = now.AddDays(-38),
            UpdatedAt = now.AddDays(-12)
        };
        var out4 = new Outcome
        {
            Description = "Auto-generate compliance reports, reducing report compilation from weeks to minutes.",
            Priority = Priority.High,
            Context = "Critical for maintaining government partnership and shelter licensing.",
            ExternalEntityId = animalControl.Id,
            CreatedAt = now.AddDays(-37),
            UpdatedAt = now.AddDays(-20)
        };
        var out5 = new Outcome
        {
            Description = "Increase adopter-to-pet match rate by 50% with intelligent search & matching.",
            Priority = Priority.Medium,
            Context = "Better matches lead to fewer returned animals.",
            ExternalEntityId = adopter.Id,
            CreatedAt = now.AddDays(-36),
            UpdatedAt = now.AddDays(-12)
        };
        context.Outcomes.AddRange(out1, out2, out3, out4, out5);
        context.SaveChanges();

        // ── Outcome Tags ────────────────────────────────────────────
        context.Set<OutcomeTag>().AddRange(
            new OutcomeTag { OutcomeId = out1.Id, TagId = tagShelter.Id },
            new OutcomeTag { OutcomeId = out2.Id, TagId = tagIntegration.Id },
            new OutcomeTag { OutcomeId = out3.Id, TagId = tagAdopter.Id },
            new OutcomeTag { OutcomeId = out4.Id, TagId = tagCompliance.Id },
            new OutcomeTag { OutcomeId = out5.Id, TagId = tagUx.Id }
        );
        context.SaveChanges();

        // ── Outcome ↔ Problem links (many-to-many) ─────────────────
        out1.Problems.Add(prob1);
        out2.Problems.Add(prob2);
        out3.Problems.Add(prob3);
        out4.Problems.Add(prob4);
        out5.Problems.Add(prob5);
        // Cross-links
        out1.Problems.Add(prob4); // Automating intake also helps compliance reporting
        out5.Problems.Add(prob3); // Better matching also reduces post-adoption issues
        context.SaveChanges();

        // ── Success Metrics ─────────────────────────────────────────
        var metrics = new List<SuccessMetric>
        {
            new() { Description = "Average data-entry time per animal listing", TargetValue = "10", CurrentValue = "55", Unit = "minutes", OutcomeId = out1.Id, CreatedAt = now.AddDays(-35), UpdatedAt = now },
            new() { Description = "Percentage of listings with verified health records", TargetValue = "95", CurrentValue = "12", Unit = "%", OutcomeId = out2.Id, CreatedAt = now.AddDays(-35), UpdatedAt = now },
            new() { Description = "Post-adoption check-in completion rate", TargetValue = "90", CurrentValue = "25", Unit = "%", OutcomeId = out3.Id, CreatedAt = now.AddDays(-35), UpdatedAt = now },
            new() { Description = "Time to generate quarterly compliance report", TargetValue = "5", CurrentValue = "10080", Unit = "minutes", OutcomeId = out4.Id, CreatedAt = now.AddDays(-35), UpdatedAt = now },
            new() { Description = "Adopter-to-pet match success rate", TargetValue = "75", CurrentValue = "50", Unit = "%", OutcomeId = out5.Id, CreatedAt = now.AddDays(-35), UpdatedAt = now }
        };
        context.SuccessMetrics.AddRange(metrics);
        context.SaveChanges();

        // ── Interviews ──────────────────────────────────────────────
        var interview1 = new Interview
        {
            Type = InterviewType.Discovery,
            InterviewDate = now.AddDays(-45),
            Interviewer = "Alice Rivera",
            Summary = "Walked through a typical day at Happy Paws. Maria showed the paper intake process and highlighted bottlenecks.",
            ExternalEntityId = shelterDirector.Id,
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-45),
            UpdatedAt = now.AddDays(-45)
        };
        var interview2 = new Interview
        {
            Type = InterviewType.Discovery,
            InterviewDate = now.AddDays(-43),
            Interviewer = "Alice Rivera",
            Summary = "Discussed vet-record sharing challenges. Greenfield uses VetLink EHR – API available but not used by shelters.",
            ExternalEntityId = vetClinic.Id,
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-43),
            UpdatedAt = now.AddDays(-43)
        };
        var interview3 = new Interview
        {
            Type = InterviewType.Feedback,
            InterviewDate = now.AddDays(-30),
            Interviewer = "Alice Rivera",
            Summary = "James shared his adoption experience. Loved the idea of post-adoption check-ins via app notifications.",
            ExternalEntityId = adopter.Id,
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-30),
            UpdatedAt = now.AddDays(-30)
        };
        context.Interviews.AddRange(interview1, interview2, interview3);
        context.SaveChanges();

        // ── Interview Notes ─────────────────────────────────────────
        context.InterviewNotes.AddRange(
            new InterviewNote { Content = "Maria estimates 55 minutes per animal for data entry including photos, medical notes, and temperament assessment.", InterviewId = interview1.Id, RelatedProblemId = prob1.Id, CreatedAt = now.AddDays(-45), UpdatedAt = now.AddDays(-45) },
            new InterviewNote { Content = "Happy Paws processes ~15 animals per week. That's nearly 14 hours/week on data entry alone.", InterviewId = interview1.Id, RelatedProblemId = prob1.Id, CreatedAt = now.AddDays(-45), UpdatedAt = now.AddDays(-45) },
            new InterviewNote { Content = "Maria would love a tablet-based intake form with camera integration for photos.", InterviewId = interview1.Id, RelatedOutcomeId = out1.Id, CreatedAt = now.AddDays(-45), UpdatedAt = now.AddDays(-45) },
            new InterviewNote { Content = "VetLink EHR has a REST API that can push vaccination records. Greenfield is willing to be a pilot partner.", InterviewId = interview2.Id, RelatedOutcomeId = out2.Id, CreatedAt = now.AddDays(-43), UpdatedAt = now.AddDays(-43) },
            new InterviewNote { Content = "James said he would have appreciated knowing his dog's full medical history before adoption day.", InterviewId = interview3.Id, RelatedProblemId = prob2.Id, CreatedAt = now.AddDays(-30), UpdatedAt = now.AddDays(-30) },
            new InterviewNote { Content = "James suggested a simple weekly check-in survey with photo upload to track pet adjustment.", InterviewId = interview3.Id, RelatedOutcomeId = out3.Id, CreatedAt = now.AddDays(-30), UpdatedAt = now.AddDays(-30) }
        );
        context.SaveChanges();

        // ══════════════════════════════════════════════════════════════
        //  USER STORY MAP
        // ══════════════════════════════════════════════════════════════

        // ── Teams ───────────────────────────────────────────────────
        var teamAlpha = new Team
        {
            Name = "Team Alpha",
            Description = "Full-stack team focused on shelter-facing features and intake automation.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-50),
            UpdatedAt = now
        };
        var teamBeta = new Team
        {
            Name = "Team Beta",
            Description = "Full-stack team focused on adopter experience, matching, and post-adoption.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-50),
            UpdatedAt = now
        };
        context.Teams.AddRange(teamAlpha, teamBeta);
        context.SaveChanges();

        // ── Releases ────────────────────────────────────────────────
        var releaseMvp = new Release
        {
            Name = "v1.0 – MVP",
            Description = "Core adoption listing, search, and shelter intake automation.",
            StartDate = now.AddDays(-30),
            ReleaseDate = now.AddDays(30),
            Status = "InProgress",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-30),
            UpdatedAt = now
        };
        var releaseV2 = new Release
        {
            Name = "v2.0 – Health & Compliance",
            Description = "Vet integration, compliance reporting, and post-adoption features.",
            StartDate = now.AddDays(31),
            ReleaseDate = now.AddDays(90),
            Status = "Planned",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-30),
            UpdatedAt = now
        };
        context.Releases.AddRange(releaseMvp, releaseV2);
        context.SaveChanges();

        // ── Sprints ─────────────────────────────────────────────────
        var sprint1 = new Sprint
        {
            Name = "Sprint 1 – Foundation",
            Goal = "Set up core data models, shelter intake form, and basic pet listing page.",
            StartDate = now.AddDays(-28),
            EndDate = now.AddDays(-15),
            Status = "Completed",
            PlanningOneNotes = "Agreed on data model for animals, shelters, and adopters. Decided on PostgreSQL + EF Core.",
            ReviewNotes = "Demonstrated working intake form and basic listing grid. Stakeholders pleased with speed.",
            RetroNotes = "Need better test coverage. Agreed to add integration tests from Sprint 2.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-30),
            UpdatedAt = now.AddDays(-14)
        };
        var sprint2 = new Sprint
        {
            Name = "Sprint 2 – Search & Profiles",
            Goal = "Implement advanced search filters, pet profile pages, and shelter dashboard.",
            StartDate = now.AddDays(-14),
            EndDate = now.AddDays(-1),
            Status = "Completed",
            PlanningOneNotes = "Prioritized search UX. Alpha takes shelter dashboard, Beta takes adopter search.",
            ReviewNotes = "Search filters working. Shelter dashboard has basic analytics. Profile pages look great.",
            RetroNotes = "Deployment pipeline needs CI/CD improvements. Stories were well-sized this sprint.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-16),
            UpdatedAt = now
        };
        var sprint3 = new Sprint
        {
            Name = "Sprint 3 – Matching & Applications",
            Goal = "Build pet-adopter matching algorithm, adoption application workflow, and notification system.",
            StartDate = now,
            EndDate = now.AddDays(13),
            Status = "Active",
            PlanningOneNotes = "Sprint goal: get the matching MVP and application flow working end-to-end. Alpha handles matching backend, Beta handles application UI and notifications.",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-2),
            UpdatedAt = now
        };
        var sprint4 = new Sprint
        {
            Name = "Sprint 4 – Polish & Launch",
            Goal = "Bug fixes, performance optimization, and MVP launch preparation.",
            StartDate = now.AddDays(14),
            EndDate = now.AddDays(27),
            Status = "Planned",
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-2),
            UpdatedAt = now
        };
        context.Sprints.AddRange(sprint1, sprint2, sprint3, sprint4);
        context.SaveChanges();

        // ── Team Plannings (Sprint Planning 2 per team) ─────────────
        context.TeamPlannings.AddRange(
            new TeamPlanning { SprintId = sprint3.Id, TeamId = teamAlpha.Id, PlanningTwoNotes = "Alpha will implement the matching scoring engine and admin configuration UI. Estimated capacity: 24 points.", CreatedAt = now, UpdatedAt = now },
            new TeamPlanning { SprintId = sprint3.Id, TeamId = teamBeta.Id, PlanningTwoNotes = "Beta will build the adoption application form, status tracker, and email notifications. Estimated capacity: 21 points.", CreatedAt = now, UpdatedAt = now }
        );
        context.SaveChanges();

        // ── Themes ──────────────────────────────────────────────────
        var themeIntake = new Theme
        {
            Name = "Shelter Intake & Management",
            Description = "Everything related to how shelters onboard animals and manage listings.",
            Order = 1,
            ProjectId = project.Id,
            OutcomeId = out1.Id,
            CreatedAt = now.AddDays(-35),
            UpdatedAt = now
        };
        var themeDiscovery = new Theme
        {
            Name = "Pet Discovery & Matching",
            Description = "How adopters find, filter, and get matched with pets.",
            Order = 2,
            ProjectId = project.Id,
            OutcomeId = out5.Id,
            CreatedAt = now.AddDays(-35),
            UpdatedAt = now
        };
        var themeAdoption = new Theme
        {
            Name = "Adoption Process",
            Description = "Application submission, review, approval, and handoff workflows.",
            Order = 3,
            ProjectId = project.Id,
            CreatedAt = now.AddDays(-35),
            UpdatedAt = now
        };
        var themePostAdoption = new Theme
        {
            Name = "Post-Adoption Support",
            Description = "Follow-up check-ins, health tracking, and community features.",
            Order = 4,
            ProjectId = project.Id,
            OutcomeId = out3.Id,
            CreatedAt = now.AddDays(-35),
            UpdatedAt = now
        };
        var themeCompliance = new Theme
        {
            Name = "Reporting & Compliance",
            Description = "Government reporting, analytics dashboards, and audit trails.",
            Order = 5,
            ProjectId = project.Id,
            OutcomeId = out4.Id,
            CreatedAt = now.AddDays(-35),
            UpdatedAt = now
        };
        context.Themes.AddRange(themeIntake, themeDiscovery, themeAdoption, themePostAdoption, themeCompliance);
        context.SaveChanges();

        // ── Epics ───────────────────────────────────────────────────
        // Theme: Shelter Intake & Management
        var epicIntakeForm = new Epic { Name = "Animal Intake Form", Description = "Digital intake form replacing paper process.", Order = 1, ThemeId = themeIntake.Id, OutcomeId = out1.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicListingMgmt = new Epic { Name = "Listing Management", Description = "CRUD for animal listings with photo upload and status management.", Order = 2, ThemeId = themeIntake.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicShelterDash = new Epic { Name = "Shelter Dashboard", Description = "Overview dashboard for shelter staff with key metrics.", Order = 3, ThemeId = themeIntake.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };

        // Theme: Pet Discovery & Matching
        var epicSearch = new Epic { Name = "Advanced Search", Description = "Multi-faceted search with filters for species, breed, temperament, energy level, and compatibility.", Order = 1, ThemeId = themeDiscovery.Id, OutcomeId = out5.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicProfiles = new Epic { Name = "Pet Profiles", Description = "Detailed pet profile pages with photos, bio, medical summary, and compatibility info.", Order = 2, ThemeId = themeDiscovery.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicMatching = new Epic { Name = "Smart Matching", Description = "Algorithm that suggests compatible pets based on adopter preferences and lifestyle.", Order = 3, ThemeId = themeDiscovery.Id, OutcomeId = out5.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };

        // Theme: Adoption Process
        var epicApplication = new Epic { Name = "Adoption Application", Description = "Online application form with document upload and references.", Order = 1, ThemeId = themeAdoption.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicReview = new Epic { Name = "Application Review", Description = "Shelter staff review workflow with approval/rejection and notes.", Order = 2, ThemeId = themeAdoption.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicNotifications = new Epic { Name = "Notifications", Description = "Email and in-app notifications for application status updates.", Order = 3, ThemeId = themeAdoption.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };

        // Theme: Post-Adoption Support
        var epicCheckins = new Epic { Name = "Check-in Surveys", Description = "Scheduled post-adoption surveys with photo upload.", Order = 1, ThemeId = themePostAdoption.Id, OutcomeId = out3.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicHealthTrack = new Epic { Name = "Health Record Integration", Description = "Pull vet records via API and display on pet profile.", Order = 2, ThemeId = themePostAdoption.Id, OutcomeId = out2.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };

        // Theme: Reporting & Compliance
        var epicReporting = new Epic { Name = "Compliance Reports", Description = "Auto-generated quarterly reports for animal control.", Order = 1, ThemeId = themeCompliance.Id, OutcomeId = out4.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };
        var epicAnalytics = new Epic { Name = "Analytics Dashboard", Description = "Adoption trends, shelter capacity, and outcome tracking.", Order = 2, ThemeId = themeCompliance.Id, CreatedAt = now.AddDays(-33), UpdatedAt = now };

        context.Epics.AddRange(epicIntakeForm, epicListingMgmt, epicShelterDash, epicSearch, epicProfiles, epicMatching, epicApplication, epicReview, epicNotifications, epicCheckins, epicHealthTrack, epicReporting, epicAnalytics);
        context.SaveChanges();

        // ── Stories ─────────────────────────────────────────────────
        // Helper
        Story S(string title, string desc, string? solution, string? ac, int order, Priority pri, string status, int epicId, int? sprintId, int? teamId, int? releaseId, int? storyPoints, int? outcomeId = null, string? assigneeId = null, string? assigneeName = null)
        {
            return new Story
            {
                Title = title,
                Description = desc,
                SolutionDescription = solution,
                AcceptanceCriteria = ac,
                Order = order,
                Priority = pri,
                Status = status,
                StoryPoints = storyPoints,
                EpicId = epicId,
                SprintId = sprintId,
                TeamId = teamId,
                ReleaseId = releaseId,
                OutcomeId = outcomeId,
                AssigneeId = assigneeId,
                AssigneeName = assigneeName,
                CreatedAt = now.AddDays(-30),
                UpdatedAt = now
            };
        }

        var stories = new List<Story>
        {
            // ── Epic: Animal Intake Form  (Sprint 1 – Done) ──────────
            S("Create animal data model & migration", "Define the core Animal entity with all fields needed for intake.", "EF Core entity with PostgreSQL migration.", "Migration runs cleanly; all fields present in DB.", 1, Priority.Critical, "Done", epicIntakeForm.Id, sprint1.Id, teamAlpha.Id, releaseMvp.Id, 3, out1.Id, "user-002", "Bob Chen"),
            S("Build digital intake form UI", "Tablet-friendly intake form with camera integration for photos.", "Vue 3 form component with image capture.", "Staff can fill intake form on tablet and capture photos inline.", 2, Priority.Critical, "Done", epicIntakeForm.Id, sprint1.Id, teamAlpha.Id, releaseMvp.Id, 8, out1.Id, "user-003", "Carol Nguyen"),
            S("Implement intake form API endpoint", "REST API to receive intake form submissions.", "ASP.NET Core controller with validation.", "POST /api/animals returns 201 with created animal; validation errors return 400.", 3, Priority.Critical, "Done", epicIntakeForm.Id, sprint1.Id, teamAlpha.Id, releaseMvp.Id, 5, out1.Id, "user-002", "Bob Chen"),

            // ── Epic: Listing Management  (Sprint 1 – Done) ─────────
            S("Animal listing grid with sorting & pagination", "Display all animals in a sortable, paginated grid.", "Vue 3 data table with server-side pagination.", "Grid shows 20 animals per page; sortable by name, date, status.", 1, Priority.High, "Done", epicListingMgmt.Id, sprint1.Id, teamAlpha.Id, releaseMvp.Id, 5, null, "user-003", "Carol Nguyen"),
            S("Photo upload & gallery for listings", "Allow multiple photo uploads per animal with drag-and-drop.", "File upload API + Vue gallery component.", "Up to 10 photos per animal; drag to reorder; delete supported.", 2, Priority.Medium, "Done", epicListingMgmt.Id, sprint1.Id, teamAlpha.Id, releaseMvp.Id, 5, null, "user-002", "Bob Chen"),
            S("Listing status workflow (Available → Pending → Adopted)", "Status transitions with audit trail.", "State machine in backend; status badge in UI.", "Staff can move animals through workflow; history tracked.", 3, Priority.High, "Done", epicListingMgmt.Id, sprint2.Id, teamAlpha.Id, releaseMvp.Id, 3, null, "user-002", "Bob Chen"),

            // ── Epic: Shelter Dashboard  (Sprint 2 – Done) ──────────
            S("Dashboard layout with key metrics cards", "Show total animals, adoption rate, avg time to adopt.", "Vue dashboard with chart.js cards.", "Dashboard loads in < 2s; shows 4 KPI cards.", 1, Priority.Medium, "Done", epicShelterDash.Id, sprint2.Id, teamAlpha.Id, releaseMvp.Id, 5, null, "user-003", "Carol Nguyen"),
            S("Recent activity feed on dashboard", "Show latest intake, adoption, and status changes.", "Server-sent events for real-time feed.", "Feed shows last 20 events; updates in real-time.", 2, Priority.Low, "Done", epicShelterDash.Id, sprint2.Id, teamAlpha.Id, releaseMvp.Id, 3, null, "user-002", "Bob Chen"),

            // ── Epic: Advanced Search  (Sprint 2 – Done) ────────────
            S("Search API with multi-faceted filters", "Support filtering by species, breed, age, size, temperament, energy.", "Full-text search with PostgreSQL + filter predicates.", "GET /api/animals/search returns filtered, paginated results.", 1, Priority.High, "Done", epicSearch.Id, sprint2.Id, teamBeta.Id, releaseMvp.Id, 8, out5.Id, "user-002", "Bob Chen"),
            S("Search UI with filter sidebar", "Responsive filter panel with checkboxes, sliders, and chips.", "Vue 3 sidebar component with reactive filters.", "Filters update results in real-time; mobile-friendly collapsible sidebar.", 2, Priority.High, "Done", epicSearch.Id, sprint2.Id, teamBeta.Id, releaseMvp.Id, 5, out5.Id, "user-003", "Carol Nguyen"),
            S("Save & share search filters", "Adopters can save filter presets and share via URL.", null, "Saved filters persist across sessions; shareable link works.", 3, Priority.Low, "Backlog", epicSearch.Id, null, null, releaseMvp.Id, 3, out5.Id),

            // ── Epic: Pet Profiles  (Sprint 2 – Done) ───────────────
            S("Pet profile page with photo carousel", "Detailed profile page with full bio, photos, and details.", "Vue profile page with Swiper carousel.", "Profile page loads in < 1.5s; carousel swipes smoothly.", 1, Priority.High, "Done", epicProfiles.Id, sprint2.Id, teamBeta.Id, releaseMvp.Id, 5, null, "user-003", "Carol Nguyen"),
            S("Compatibility info section on profiles", "Show compatibility scores for living situation, children, other pets.", "Algorithm-driven badges on profile.", "Compatibility section visible with 3+ badges per animal.", 2, Priority.Medium, "Done", epicProfiles.Id, sprint2.Id, teamBeta.Id, releaseMvp.Id, 5, out5.Id, "user-002", "Bob Chen"),

            // ── Epic: Smart Matching  (Sprint 3 – Active) ───────────
            S("Define matching algorithm scoring model", "Design the scoring weights for lifestyle-to-pet matching.", "Weighted scoring model with configurable weights in admin.", "Scoring model documented; weights adjustable via admin panel.", 1, Priority.Critical, "In Progress", epicMatching.Id, sprint3.Id, teamAlpha.Id, releaseMvp.Id, 8, out5.Id, "user-002", "Bob Chen"),
            S("Build matching API endpoint", "API that returns top-N matched pets for an adopter profile.", "Matching service with scoring + pagination.", "GET /api/match returns ranked results with scores.", 2, Priority.Critical, "In Progress", epicMatching.Id, sprint3.Id, teamAlpha.Id, releaseMvp.Id, 8, out5.Id, "user-002", "Bob Chen"),
            S("Matching results UI with explanations", "Show matched pets with match % and reasoning.", "Vue card grid with match percentage badge and tooltip.", "Matched pets shown with % score; hover shows breakdown.", 3, Priority.High, "To Do", epicMatching.Id, sprint3.Id, teamAlpha.Id, releaseMvp.Id, 5, out5.Id, "user-003", "Carol Nguyen"),
            S("Admin panel for matching weight configuration", "Let product managers tweak matching algorithm weights.", null, "Admin can adjust weights; changes reflect immediately in results.", 4, Priority.Medium, "Backlog", epicMatching.Id, null, null, releaseMvp.Id, 3, out5.Id),

            // ── Epic: Adoption Application  (Sprint 3 – Active) ─────
            S("Adoption application form", "Multi-step form with personal info, living situation, references.", "Vue multi-step wizard with validation.", "All steps completable; validation on each step; submit works.", 1, Priority.Critical, "In Progress", epicApplication.Id, sprint3.Id, teamBeta.Id, releaseMvp.Id, 8, null, "user-003", "Carol Nguyen"),
            S("Application submission API", "REST endpoint to submit and store applications.", "Controller with DTO validation and DB persistence.", "POST /api/applications returns 201; stores all form data.", 2, Priority.Critical, "To Do", epicApplication.Id, sprint3.Id, teamBeta.Id, releaseMvp.Id, 5, null, "user-002", "Bob Chen"),
            S("Application status tracker for adopters", "Adopters can view their application status in real-time.", "Vue status timeline component.", "Timeline shows Submitted → Under Review → Approved/Rejected.", 3, Priority.High, "To Do", epicApplication.Id, sprint3.Id, teamBeta.Id, releaseMvp.Id, 5, null),

            // ── Epic: Application Review  (Sprint 4 – Planned) ──────
            S("Review queue for shelter staff", "List of pending applications with sorting and quick actions.", null, "Staff see all pending applications; can sort by date, pet.", 1, Priority.High, "Backlog", epicReview.Id, null, null, releaseMvp.Id, 5),
            S("Application detail view with approve/reject", "Detailed view with applicant info, notes field, and decision buttons.", null, "Staff can approve or reject with required notes.", 2, Priority.High, "Backlog", epicReview.Id, null, null, releaseMvp.Id, 5),

            // ── Epic: Notifications  (Sprint 3 – Active) ────────────
            S("Email notification service", "Send emails on application status changes.", "SendGrid integration with templated emails.", "Emails sent within 1 min of status change; correct template used.", 1, Priority.High, "To Do", epicNotifications.Id, sprint3.Id, teamBeta.Id, releaseMvp.Id, 5, null, "user-003", "Carol Nguyen"),
            S("In-app notification bell", "Real-time notification bell with unread count.", null, "Bell shows count; clicking opens notification list.", 2, Priority.Medium, "Backlog", epicNotifications.Id, null, null, releaseMvp.Id, 3),

            // ── Epic: Check-in Surveys  (v2.0) ──────────────────────
            S("Post-adoption check-in survey form", "Weekly survey with happiness rating, concerns, and photo upload.", null, "Survey submittable with all fields; photos stored.", 1, Priority.Medium, "Backlog", epicCheckins.Id, null, null, releaseV2.Id, 5, out3.Id),
            S("Automated check-in reminders", "Send push/email reminders at 1, 7, 14, 30 days post-adoption.", null, "Reminders sent on schedule; adopter can snooze.", 2, Priority.Medium, "Backlog", epicCheckins.Id, null, null, releaseV2.Id, 3, out3.Id),

            // ── Epic: Health Record Integration  (v2.0) ─────────────
            S("VetLink EHR API integration", "Connect to VetLink REST API to pull vaccination records.", null, "Records pulled on demand; displayed on pet profile.", 1, Priority.High, "Backlog", epicHealthTrack.Id, null, null, releaseV2.Id, 8, out2.Id),
            S("Health record timeline on pet profile", "Visual timeline showing vaccinations, check-ups, and treatments.", null, "Timeline renders chronologically; clicking expands details.", 2, Priority.Medium, "Backlog", epicHealthTrack.Id, null, null, releaseV2.Id, 5, out2.Id),

            // ── Epic: Compliance Reports  (v2.0) ────────────────────
            S("Quarterly report generator", "Auto-generate PDF reports with adoption stats for animal control.", null, "PDF generated with all required fields; downloadable.", 1, Priority.High, "Backlog", epicReporting.Id, null, null, releaseV2.Id, 8, out4.Id),
            S("Report scheduling & email delivery", "Schedule reports to auto-send to government contacts.", null, "Reports sent on schedule; delivery confirmed via email.", 2, Priority.Medium, "Backlog", epicReporting.Id, null, null, releaseV2.Id, 5, out4.Id),

            // ── Epic: Analytics Dashboard  (v2.0) ───────────────────
            S("Adoption trends chart", "Line/bar charts showing adoption rates over time.", null, "Charts render with real data; filterable by date range.", 1, Priority.Low, "Backlog", epicAnalytics.Id, null, null, releaseV2.Id, 5),
            S("Shelter capacity heatmap", "Visual heatmap showing capacity utilization across shelters.", null, "Heatmap renders for all shelters; color-coded by utilization.", 2, Priority.Low, "Backlog", epicAnalytics.Id, null, null, releaseV2.Id, 5)
        };
        context.Stories.AddRange(stories);
        context.SaveChanges();

        // ── Spikes ──────────────────────────────────────────────────
        var spikes = new List<Spike>
        {
            new()
            {
                Title = "Evaluate matching algorithm approaches",
                Description = "Research collaborative filtering vs. rule-based vs. ML approaches for pet-adopter matching.",
                InvestigationGoal = "Determine the best approach for MVP matching given time constraints and data availability.",
                Findings = "Rule-based with weighted scoring is best for MVP. ML requires more training data than we have. Collaborative filtering needs usage history we don't have yet. Recommended: weighted scoring now, ML in v2.",
                Order = 1, Priority = Priority.Critical, Status = "Done", StoryPoints = 5,
                EpicId = epicMatching.Id, SprintId = sprint2.Id, TeamId = teamAlpha.Id, ReleaseId = releaseMvp.Id,
                OutcomeId = out5.Id, AssigneeId = "user-002", AssigneeName = "Bob Chen",
                CreatedAt = now.AddDays(-20), UpdatedAt = now.AddDays(-8)
            },
            new()
            {
                Title = "VetLink EHR API feasibility study",
                Description = "Investigate VetLink API capabilities, authentication, rate limits, and data format.",
                InvestigationGoal = "Confirm we can pull vaccination records reliably and determine integration effort.",
                Findings = null, // Not started yet
                Order = 1, Priority = Priority.High, Status = "Backlog", StoryPoints = 3,
                EpicId = epicHealthTrack.Id, ReleaseId = releaseV2.Id,
                OutcomeId = out2.Id,
                CreatedAt = now.AddDays(-15), UpdatedAt = now.AddDays(-15)
            },
            new()
            {
                Title = "Evaluate email service providers",
                Description = "Compare SendGrid, Mailgun, and AWS SES for transactional email delivery.",
                InvestigationGoal = "Select the best email provider for cost, deliverability, and template support.",
                Findings = null,
                Order = 1, Priority = Priority.Medium, Status = "In Progress", StoryPoints = 2,
                EpicId = epicNotifications.Id, SprintId = sprint3.Id, TeamId = teamBeta.Id, ReleaseId = releaseMvp.Id,
                AssigneeId = "user-003", AssigneeName = "Carol Nguyen",
                CreatedAt = now.AddDays(-5), UpdatedAt = now
            }
        };
        context.Spikes.AddRange(spikes);
        context.SaveChanges();
    }
}
