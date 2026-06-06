using linkedin_api.Models;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<PremiumSubscription> PremiumSubscriptions => Set<PremiumSubscription>();
    public DbSet<PremiumFeatureGate> PremiumFeatureGates => Set<PremiumFeatureGate>();
    public DbSet<InMailUsage> InMailUsages => Set<InMailUsage>();
    public DbSet<ProfileView> ProfileViews => Set<ProfileView>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Endorsement> Endorsements => Set<Endorsement>();
    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<VolunteerExperience> VolunteerExperiences => Set<VolunteerExperience>();
    public DbSet<HonorAward> HonorAwards => Set<HonorAward>();
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<Patent> Patents => Set<Patent>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<FeaturedItem> FeaturedItems => Set<FeaturedItem>();
    public DbSet<Connection> Connections => Set<Connection>();
    public DbSet<ConnectionRequest> ConnectionRequests => Set<ConnectionRequest>();
    public DbSet<ConnectionSuggestion> ConnectionSuggestions => Set<ConnectionSuggestion>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Block> Blocks => Set<Block>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<SavedItem> SavedItems => Set<SavedItem>();
    public DbSet<Hashtag> Hashtags => Set<Hashtag>();
    public DbSet<PostReport> PostReports => Set<PostReport>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<PushToken> PushTokens => Set<PushToken>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<JobAlert> JobAlerts => Set<JobAlert>();
    public DbSet<JobRecommendation> JobRecommendations => Set<JobRecommendation>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyAdmin> CompanyAdmins => Set<CompanyAdmin>();
    public DbSet<CompanyProduct> CompanyProducts => Set<CompanyProduct>();
    public DbSet<CompanyFollow> CompanyFollows => Set<CompanyFollow>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<DailyPuzzle> DailyPuzzles => Set<DailyPuzzle>();
    public DbSet<PuzzleAttempt> PuzzleAttempts => Set<PuzzleAttempt>();
    public DbSet<PuzzleStreak> PuzzleStreaks => Set<PuzzleStreak>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
    public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Connection>().HasIndex(x => new { x.UserId, x.ConnectedUserId }).IsUnique();
        modelBuilder.Entity<Follow>().HasIndex(x => new { x.FollowerUserId, x.FollowedUserId }).IsUnique();
        modelBuilder.Entity<Block>().HasIndex(x => new { x.BlockerUserId, x.BlockedUserId }).IsUnique();
        modelBuilder.Entity<CompanyFollow>().HasIndex(x => new { x.CompanyId, x.UserId }).IsUnique();
        modelBuilder.Entity<GroupMember>().HasIndex(x => new { x.GroupId, x.UserId }).IsUnique();
        modelBuilder.Entity<DailyPuzzle>().HasIndex(x => new { x.Type, x.PuzzleDate }).IsUnique();
        modelBuilder.Entity<UserSettings>().HasIndex(x => x.UserId).IsUnique();
    }
}
