using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using linkedin_api.Enums;

namespace linkedin_api.Models;

public abstract class EntityBase
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class User : EntityBase
{
    [MaxLength(256)] public string Email { get; set; } = "";
    [MaxLength(512)] public string PasswordHash { get; set; } = "";
    [MaxLength(80)] public string FirstName { get; set; } = "";
    [MaxLength(80)] public string LastName { get; set; } = "";
    [MaxLength(180)] public string Headline { get; set; } = "";
    [MaxLength(160)] public string Location { get; set; } = "";
    [MaxLength(600)] public string? AvatarUrl { get; set; }
    [MaxLength(600)] public string? BannerUrl { get; set; }
    [Column(TypeName = "text")] public string? Summary { get; set; }
    public bool OpenToWork { get; set; }
    [Column(TypeName = "json")] public string? OpenToWorkConfigJson { get; set; }
    public AccountTier AccountTier { get; set; } = AccountTier.Free;
    public ProfileVisibility ProfileVisibility { get; set; } = ProfileVisibility.Public;
    public bool MfaEnabled { get; set; }
    [MaxLength(128)] public string? MfaSecret { get; set; }
    [MaxLength(512)] public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PremiumSubscription : EntityBase { public int UserId { get; set; } public AccountTier Tier { get; set; } public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active; public DateTime StartsAt { get; set; } = DateTime.UtcNow; public DateTime? ExpiresAt { get; set; } public bool AutoRenew { get; set; } = true; }
public class PremiumFeatureGate : EntityBase { [MaxLength(120)] public string FeatureKey { get; set; } = ""; public AccountTier MinimumTier { get; set; } = AccountTier.PremiumCareer; public bool Enabled { get; set; } = true; [MaxLength(600)] public string? Description { get; set; } }
public class InMailUsage : EntityBase { public int UserId { get; set; } public int CreditsGranted { get; set; } public int CreditsUsed { get; set; } public DateTime PeriodStart { get; set; } = DateTime.UtcNow.Date; public DateTime PeriodEnd { get; set; } = DateTime.UtcNow.Date.AddMonths(1); }
public class ProfileView : EntityBase { public int ViewedUserId { get; set; } public int ViewerUserId { get; set; } public bool Anonymous { get; set; } }

public class Experience : EntityBase { public int UserId { get; set; } [MaxLength(160)] public string Title { get; set; } = ""; [MaxLength(160)] public string Company { get; set; } = ""; [MaxLength(120)] public string? Location { get; set; } public DateTime? StartDate { get; set; } public DateTime? EndDate { get; set; } public bool Current { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class Education : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string School { get; set; } = ""; [MaxLength(160)] public string? Degree { get; set; } [MaxLength(160)] public string? FieldOfStudy { get; set; } public DateTime? StartDate { get; set; } public DateTime? EndDate { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class Skill : EntityBase { public int UserId { get; set; } [MaxLength(120)] public string Name { get; set; } = ""; public int EndorsementCount { get; set; } }
public class Endorsement : EntityBase { public int SkillId { get; set; } public int EndorserUserId { get; set; } public int UserId { get; set; } }
public class Certification : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string Name { get; set; } = ""; [MaxLength(180)] public string? Issuer { get; set; } public DateTime? IssuedAt { get; set; } public DateTime? ExpiresAt { get; set; } [MaxLength(240)] public string? CredentialUrl { get; set; } }
public class VolunteerExperience : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string Organization { get; set; } = ""; [MaxLength(160)] public string? Role { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class HonorAward : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string Title { get; set; } = ""; [MaxLength(180)] public string? Issuer { get; set; } public DateTime? IssuedAt { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class Publication : EntityBase { public int UserId { get; set; } [MaxLength(220)] public string Title { get; set; } = ""; [MaxLength(240)] public string? Url { get; set; } public DateTime? PublishedAt { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class Patent : EntityBase { public int UserId { get; set; } [MaxLength(220)] public string Title { get; set; } = ""; [MaxLength(120)] public string? PatentNumber { get; set; } public DateTime? IssuedAt { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class Course : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string Name { get; set; } = ""; [MaxLength(80)] public string? Number { get; set; } }
public class Project : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string Name { get; set; } = ""; [MaxLength(240)] public string? Url { get; set; } public DateTime? StartDate { get; set; } public DateTime? EndDate { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class Language : EntityBase { public int UserId { get; set; } [MaxLength(120)] public string Name { get; set; } = ""; [MaxLength(80)] public string? Proficiency { get; set; } }
public class Recommendation : EntityBase { public int UserId { get; set; } public int RecommenderUserId { get; set; } [Column(TypeName = "text")] public string Text { get; set; } = ""; public bool Approved { get; set; } }
public class FeaturedItem : EntityBase { public int UserId { get; set; } [MaxLength(180)] public string Title { get; set; } = ""; [MaxLength(600)] public string? Url { get; set; } [MaxLength(80)] public string? ItemType { get; set; } }

public class Connection : EntityBase { public int UserId { get; set; } public int ConnectedUserId { get; set; } }
public class ConnectionRequest : EntityBase { public int RequesterUserId { get; set; } public int AddresseeUserId { get; set; } public ConnectionRequestStatus Status { get; set; } = ConnectionRequestStatus.Pending; [MaxLength(500)] public string? Message { get; set; } }
public class ConnectionSuggestion : EntityBase { public int UserId { get; set; } public int SuggestedUserId { get; set; } public SuggestionCategory Category { get; set; } public bool Dismissed { get; set; } public double Score { get; set; } }
public class Follow : EntityBase { public int FollowerUserId { get; set; } public int FollowedUserId { get; set; } }
public class Block : EntityBase { public int BlockerUserId { get; set; } public int BlockedUserId { get; set; } }

public class Post : EntityBase { public int AuthorUserId { get; set; } public int? CompanyId { get; set; } public int? GroupId { get; set; } public PostType Type { get; set; } = PostType.Text; public TemplateType TemplateType { get; set; } = TemplateType.None; [Column(TypeName = "text")] public string? Content { get; set; } [Column(TypeName = "json")] public string? MediaUrlsJson { get; set; } [MaxLength(600)] public string? LinkUrl { get; set; } public bool IsDraft { get; set; } public bool IsPinned { get; set; } public int? RepostOfPostId { get; set; } public RepostType? RepostType { get; set; } }
public class Reaction : EntityBase { public int UserId { get; set; } public int? PostId { get; set; } public int? CommentId { get; set; } public int? MessageId { get; set; } public ReactionType Type { get; set; } = ReactionType.Like; }
public class Comment : EntityBase { public int PostId { get; set; } public int UserId { get; set; } public int? ParentCommentId { get; set; } [Column(TypeName = "text")] public string Text { get; set; } = ""; }
public class SavedItem : EntityBase { public int UserId { get; set; } public SavedItemType Type { get; set; } public int ItemId { get; set; } [MaxLength(120)] public string CollectionName { get; set; } = "Default"; }
public class Hashtag : EntityBase { [MaxLength(120)] public string Tag { get; set; } = ""; public int? FollowerUserId { get; set; } public int? PostId { get; set; } }
public class PostReport : EntityBase { public int PostId { get; set; } public int ReporterUserId { get; set; } [MaxLength(120)] public string Reason { get; set; } = ""; [Column(TypeName = "text")] public string? Details { get; set; } }

public class Conversation : EntityBase { public ConversationType Type { get; set; } = ConversationType.Direct; [MaxLength(180)] public string? Title { get; set; } public bool IsMuted { get; set; } public bool IsArchived { get; set; } }
public class ConversationMember : EntityBase { public int ConversationId { get; set; } public int UserId { get; set; } public DateTime? LastReadAt { get; set; } public bool Deleted { get; set; } }
public class Message : EntityBase { public int ConversationId { get; set; } public int SenderUserId { get; set; } [Column(TypeName = "text")] public string Text { get; set; } = ""; public MessageAttachmentType AttachmentType { get; set; } public string? AttachmentUrl { get; set; } public bool Deleted { get; set; } public DateTime? ReadAt { get; set; } }
public class MessageReaction : EntityBase { public int MessageId { get; set; } public int UserId { get; set; } public ReactionType Type { get; set; } = ReactionType.Like; }

public class Notification : EntityBase { public int UserId { get; set; } public NotificationType Type { get; set; } [MaxLength(220)] public string Title { get; set; } = ""; [Column(TypeName = "text")] public string? Body { get; set; } public bool Read { get; set; } public string? LinkUrl { get; set; } }
public class NotificationPreference : EntityBase { public int UserId { get; set; } public NotificationType Type { get; set; } public bool InApp { get; set; } = true; public bool Email { get; set; } = true; public bool Push { get; set; } = true; }
public class PushToken : EntityBase { public int UserId { get; set; } [MaxLength(600)] public string Token { get; set; } = ""; [MaxLength(80)] public string? Platform { get; set; } }

public class Job : EntityBase { public int CompanyId { get; set; } public int PosterUserId { get; set; } [MaxLength(220)] public string Title { get; set; } = ""; [MaxLength(160)] public string? Location { get; set; } public JobType JobType { get; set; } public WorkplaceType WorkplaceType { get; set; } public ExperienceLevel ExperienceLevel { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } [Column(TypeName = "json")] public string? SkillsJson { get; set; } public bool IsOpen { get; set; } = true; }
public class JobApplication : EntityBase { public int JobId { get; set; } public int ApplicantUserId { get; set; } public ApplicationStage Stage { get; set; } = ApplicationStage.Applied; [MaxLength(600)] public string? ResumeUrl { get; set; } [Column(TypeName = "text")] public string? CoverLetter { get; set; } }
public class JobAlert : EntityBase { public int UserId { get; set; } [MaxLength(220)] public string Name { get; set; } = ""; [Column(TypeName = "json")] public string? FiltersJson { get; set; } public bool Enabled { get; set; } = true; }
public class JobRecommendation : EntityBase { public int UserId { get; set; } public int JobId { get; set; } public double Score { get; set; } [MaxLength(240)] public string? Reason { get; set; } }

public class Company : EntityBase { [MaxLength(220)] public string Name { get; set; } = ""; [MaxLength(160)] public string? Industry { get; set; } [MaxLength(600)] public string? LogoUrl { get; set; } [MaxLength(600)] public string? WebsiteUrl { get; set; } [Column(TypeName = "text")] public string? Description { get; set; } }
public class CompanyAdmin : EntityBase { public int CompanyId { get; set; } public int UserId { get; set; } [MaxLength(80)] public string Role { get; set; } = "Admin"; }
public class CompanyProduct : EntityBase { public int CompanyId { get; set; } [MaxLength(220)] public string Name { get; set; } = ""; [Column(TypeName = "text")] public string? Description { get; set; } [MaxLength(600)] public string? Url { get; set; } }
public class CompanyFollow : EntityBase { public int CompanyId { get; set; } public int UserId { get; set; } }

public class Group : EntityBase { [MaxLength(220)] public string Name { get; set; } = ""; public int OwnerUserId { get; set; } public GroupVisibility Visibility { get; set; } = GroupVisibility.Public; [Column(TypeName = "text")] public string? Description { get; set; } }
public class GroupMember : EntityBase { public int GroupId { get; set; } public int UserId { get; set; } public GroupRole Role { get; set; } = GroupRole.Member; public bool Approved { get; set; } = true; }

public class DailyPuzzle : EntityBase { public GameType Type { get; set; } public DateTime PuzzleDate { get; set; } = DateTime.UtcNow.Date; [Column(TypeName = "json")] public string PuzzleJson { get; set; } = "{}"; [Column(TypeName = "json")] public string? SolutionJson { get; set; } }
public class PuzzleAttempt : EntityBase { public int UserId { get; set; } public int DailyPuzzleId { get; set; } public GameType Type { get; set; } public bool Solved { get; set; } public int Attempts { get; set; } [Column(TypeName = "json")] public string? AttemptJson { get; set; } public TimeSpan? Duration { get; set; } }
public class PuzzleStreak : EntityBase { public int UserId { get; set; } public GameType Type { get; set; } public int CurrentStreak { get; set; } public int BestStreak { get; set; } public DateTime? LastPlayedAt { get; set; } }

public class UserSettings : EntityBase { public int UserId { get; set; } [Column(TypeName = "json")] public string PrivacyJson { get; set; } = "{}"; [Column(TypeName = "json")] public string NotificationsJson { get; set; } = "{}"; [Column(TypeName = "json")] public string JobSeekingJson { get; set; } = "{}"; [Column(TypeName = "json")] public string AdvertisingJson { get; set; } = "{}"; [Column(TypeName = "json")] public string DataPrivacyJson { get; set; } = "{}"; [Column(TypeName = "json")] public string AccountJson { get; set; } = "{}"; public bool Deactivated { get; set; } }

public class SearchHistory : EntityBase { public int UserId { get; set; } [MaxLength(300)] public string Query { get; set; } = ""; public SearchType Type { get; set; } = SearchType.All; }
public class SavedSearch : EntityBase { public int UserId { get; set; } [MaxLength(220)] public string Name { get; set; } = ""; [MaxLength(300)] public string Query { get; set; } = ""; public SearchType Type { get; set; } = SearchType.All; [Column(TypeName = "json")] public string? FiltersJson { get; set; } }
