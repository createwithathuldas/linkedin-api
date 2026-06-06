# LinkedIn Clone — Full Project Specification v2.0

> **Total API Endpoints:** 150+  
> **Domains:** 12  
> **Backend:** ASP.NET Core 8 Web API (C#)  
> **Stack:** .NET 8 · MySQL · Redis

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Tech Stack](#2-tech-stack)
3. [File Structure](#3-file-structure)
4. [Database Models — 60 tables](#4-database-models)
5. [Premium vs Free Tier](#5-premium-vs-free-tier)
6. [Domain 1 — Auth](#6-domain-1--auth)
7. [Domain 2 — Profile (All Sections)](#7-domain-2--profile-all-sections)
8. [Domain 3 — Connections & Network](#8-domain-3--connections--network)
9. [Domain 4 — Feed / Posts & Media](#9-domain-4--feed--posts--media)
10. [Domain 5 — Messaging](#10-domain-5--messaging)
11. [Domain 6 — Notifications](#11-domain-6--notifications)
12. [Domain 7 — Jobs](#12-domain-7--jobs)
13. [Domain 8 — Companies](#13-domain-8--companies)
14. [Domain 9 — Groups](#14-domain-9--groups)
15. [Domain 10 — Puzzle Games](#15-domain-10--puzzle-games)
16. [Domain 11 — Search](#16-domain-11--search)
17. [Domain 12 — Settings](#17-domain-12--settings)
18. [Analytics](#18-analytics)
19. [Endpoint Count Summary](#19-endpoint-count-summary)

---

## 1. Project Overview

Full-featured LinkedIn clone with Free and Premium tier separation, rich profiles, professional networking, content publishing with media templates, job marketplace, company pages, groups, real-time messaging, puzzle games, and granular settings.

### Core Feature List

| # | Feature |
|---|---------|
| 1 | Auth — email/pwd, OAuth, MFA, JWT |
| 2 | Free vs Premium user tiers with feature gating |
| 3 | Rich profiles — 14 sections (experience, education, skills, certs, patents, publications, etc.) |
| 4 | My Network — invitation manager, suggestion categories, settings |
| 5 | Feed — text/image/video/doc/link/template posts, all reactions, repost with/without thoughts, share, send |
| 6 | Real-time messaging — DMs, group chat, InMail (Premium) |
| 7 | Notifications — in-app, email, push, per-type preferences |
| 8 | Jobs — categories, personalized ML suggestions, Easy Apply, ATS pipeline |
| 9 | Company pages — admin roles, analytics, product showcase |
| 10 | Groups — public/private, moderation, member roles |
| 11 | Puzzle games — Wordle, Crossword, Tango, Queens, Pinpoint, Zip + streaks |
| 12 | Saved posts — collections, folders |
| 13 | Settings — privacy, notifications, job seeking, advertising, data, account |

---

## 2. Tech Stack

| Layer | Technology |
|-------|-----------|
| API Framework | ASP.NET Core 8 Web API (C#) |
| Primary DB | MySQL + EF Core |
| Cache / Sessions | Redis |
| Search Engine | Elasticsearch (Open-Source / Self-Hosted) |
| File Storage | Local File System Storage (stored locally in media folders) |
| Media Processing | FFmpeg transcoding pipeline |
| Real-time | SignalR (WebSocket) |
| Job Queue | Hangfire / BullMQ |
| Auth | JWT RS256 + Redis blacklist |
| Email Service | Brevo (Free Tier / SMTP) |
| CDN | None (Local Web Server Static File Hosting) |
| Container | Docker + Kubernetes |
| CI/CD | GitHub Actions |

---

## 3. File Structure

```
dotnet new webapi -n linkedin-api
cd linkedin-api
mkdir Data DTOs Models Controllers Services
```

```
linkedin-api/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── ConnectionsController.cs
│   ├── PostsController.cs
│   ├── MessagingController.cs
│   ├── NotificationsController.cs
│   ├── JobsController.cs
│   ├── CompaniesController.cs
│   ├── GroupsController.cs
│   ├── GamesController.cs
│   ├── SearchController.cs
│   ├── SettingsController.cs
│   ├── PremiumController.cs
│   └── AnalyticsController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/                          (request/response DTOs per domain)
├── Models/
│   ├── Enums.cs
│   ├── User.cs
│   ├── ProfileSections.cs
│   ├── Post.cs
│   ├── Connection.cs
│   ├── Messaging.cs
│   ├── Notification.cs
│   ├── Job.cs
│   ├── Company.cs
│   ├── Group.cs
│   ├── Games.cs
│   ├── Settings.cs
│   ├── Premium.cs
│   └── Search.cs
├── Services/                      (business logic per domain)
├── Properties/
├── Program.cs
└── linkedin-api.csproj
```

---

## 4. Database Models

### 60 Tables across 12 domains

| Domain | Models |
|--------|--------|
| Users / Auth | `User` `PremiumSubscription` `PremiumFeatureGate` `InMailUsage` `ProfileView` |
| Profile | `Experience` `Education` `Skill` `Endorsement` `Certification` `VolunteerExperience` `HonorAward` `Publication` `Patent` `Course` `Project` `Language` `Recommendation` `FeaturedItem` |
| Connections | `Connection` `ConnectionRequest` `ConnectionSuggestion` `Follow` `Block` |
| Posts | `Post` `Reaction` `Comment` `SavedItem` `Hashtag` `PostReport` |
| Messaging | `Conversation` `ConversationMember` `Message` `MessageReaction` |
| Notifications | `Notification` `NotificationPreference` `PushToken` |
| Jobs | `Job` `JobApplication` `JobAlert` `JobRecommendation` |
| Companies | `Company` `CompanyAdmin` `CompanyProduct` `CompanyFollow` |
| Groups | `Group` `GroupMember` |
| Games | `DailyPuzzle` `PuzzleAttempt` `PuzzleStreak` |
| Settings | `UserSettings` |
| Search | `SearchHistory` `SavedSearch` |

---

## 5. Premium vs Free Tier

| Feature | Free | Premium Career | Premium Business | Sales Navigator | Recruiter |
|---------|:----:|:--------------:|:----------------:|:---------------:|:---------:|
| InMail credits/month | 0 | 5 | 15 | 50 | 150 |
| Who viewed profile (full list) | ❌ | ✅ | ✅ | ✅ | ✅ |
| Profile viewers detail (90 days) | ❌ | ✅ | ✅ | ✅ | ✅ |
| Open Profile (anyone can InMail) | ❌ | ✅ | ✅ | ✅ | ✅ |
| Salary insights | ❌ | ✅ | ✅ | ✅ | ✅ |
| Company insights | ❌ | ❌ | ✅ | ✅ | ✅ |
| LinkedIn Learning | ❌ | ✅ | ✅ | ✅ | ✅ |
| Advanced search filters | ❌ | Partial | ✅ | ✅ | ✅ |
| Message non-connections | ❌ | ✅ | ✅ | ✅ | ✅ |
| Puzzle games (all) | Partial | ✅ | ✅ | ✅ | ✅ |
| Promoted job visibility | ❌ | ✅ | ✅ | ✅ | ✅ |
| Job applicant insights | ❌ | ✅ | ✅ | ✅ | ✅ |

### Premium API Endpoints

| Method | Endpoint | Tier | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/premium/plans` | ❌ | List available plans |
| POST | `/api/v1/premium/subscribe` | ✅ | Start subscription |
| PUT | `/api/v1/premium/upgrade` | ✅ | Upgrade tier |
| DELETE | `/api/v1/premium/cancel` | ✅ | Cancel subscription |
| GET | `/api/v1/premium/status` | ✅ | My current tier + expiry |
| GET | `/api/v1/premium/inmail-credits` | Premium | Remaining InMail credits |
| GET | `/api/v1/premium/features` | ✅ | Feature gate list |

**Subtotal: 7 endpoints**

*Note: Payment processing for Premium subscriptions is simulated locally or integrated with Stripe in Sandbox/Test Mode to avoid transaction fees.*

---

## 6. Domain 1 — Auth

### Requirements
- Email + password signup
- OAuth 2.0 (Google, Apple)
- JWT access token + refresh token rotation
- MFA (TOTP)
- Session management (multi-device)
- Rate-limited login

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| POST | `/api/v1/auth/register` | ❌ | Email + password signup |
| POST | `/api/v1/auth/login` | ❌ | Issue access + refresh JWT |
| POST | `/api/v1/auth/refresh` | ❌ | Rotate refresh token |
| POST | `/api/v1/auth/logout` | ✅ | Revoke token |
| POST | `/api/v1/auth/oauth/:provider` | ❌ | Google / Apple OAuth |
| POST | `/api/v1/auth/forgot-password` | ❌ | Send reset email |
| POST | `/api/v1/auth/reset-password` | ❌ | Consume reset token |
| POST | `/api/v1/auth/mfa/enable` | ✅ | TOTP setup |
| POST | `/api/v1/auth/mfa/verify` | ✅ | TOTP verify |
| DELETE | `/api/v1/auth/sessions` | ✅ | Kill all sessions |

**Subtotal: 10 endpoints**

---

## 7. Domain 2 — Profile (All Sections)

### Profile Sections (14 total)
1. Basic info (name, headline, location, avatar, banner)
2. About (summary)
3. Experience
4. Education
5. Skills + Endorsements
6. Certifications
7. Volunteer Experience
8. Honors & Awards
9. Publications
10. Patents
11. Courses
12. Projects
13. Languages
14. Recommendations

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/users/:userId` | ✅ | Get full profile |
| PUT | `/api/v1/users/:userId` | ✅ | Update basic info |
| POST | `/api/v1/users/:userId/avatar` | ✅ | Upload avatar |
| POST | `/api/v1/users/:userId/banner` | ✅ | Upload banner |
| PUT | `/api/v1/users/:userId/open-to-work` | ✅ | Toggle OTW + config |
| PUT | `/api/v1/users/:userId/privacy` | ✅ | Set visibility |
| GET | `/api/v1/users/:userId/profile-views` | ✅ (Premium: full) | Who viewed |
| POST | `/api/v1/users/:userId/experience` | ✅ | Add experience |
| PUT | `/api/v1/users/:userId/experience/:id` | ✅ | Edit experience |
| DELETE | `/api/v1/users/:userId/experience/:id` | ✅ | Delete experience |
| POST | `/api/v1/users/:userId/education` | ✅ | Add education |
| PUT | `/api/v1/users/:userId/education/:id` | ✅ | Edit education |
| DELETE | `/api/v1/users/:userId/education/:id` | ✅ | Delete education |
| POST | `/api/v1/users/:userId/skills` | ✅ | Add skill |
| DELETE | `/api/v1/users/:userId/skills/:id` | ✅ | Delete skill |
| POST | `/api/v1/users/:userId/skills/:id/endorse` | ✅ | Endorse skill |
| DELETE | `/api/v1/users/:userId/skills/:id/endorse` | ✅ | Remove endorsement |
| POST | `/api/v1/users/:userId/certifications` | ✅ | Add certification |
| PUT | `/api/v1/users/:userId/certifications/:id` | ✅ | Edit certification |
| DELETE | `/api/v1/users/:userId/certifications/:id` | ✅ | Delete certification |
| POST | `/api/v1/users/:userId/volunteer` | ✅ | Add volunteer exp |
| PUT | `/api/v1/users/:userId/volunteer/:id` | ✅ | Edit volunteer exp |
| DELETE | `/api/v1/users/:userId/volunteer/:id` | ✅ | Delete volunteer exp |
| POST | `/api/v1/users/:userId/honors` | ✅ | Add honor/award |
| PUT | `/api/v1/users/:userId/honors/:id` | ✅ | Edit honor |
| DELETE | `/api/v1/users/:userId/honors/:id` | ✅ | Delete honor |
| POST | `/api/v1/users/:userId/publications` | ✅ | Add publication |
| PUT | `/api/v1/users/:userId/publications/:id` | ✅ | Edit publication |
| DELETE | `/api/v1/users/:userId/publications/:id` | ✅ | Delete publication |
| POST | `/api/v1/users/:userId/patents` | ✅ | Add patent |
| PUT | `/api/v1/users/:userId/patents/:id` | ✅ | Edit patent |
| DELETE | `/api/v1/users/:userId/patents/:id` | ✅ | Delete patent |
| POST | `/api/v1/users/:userId/courses` | ✅ | Add course |
| DELETE | `/api/v1/users/:userId/courses/:id` | ✅ | Delete course |
| POST | `/api/v1/users/:userId/projects` | ✅ | Add project |
| PUT | `/api/v1/users/:userId/projects/:id` | ✅ | Edit project |
| DELETE | `/api/v1/users/:userId/projects/:id` | ✅ | Delete project |
| POST | `/api/v1/users/:userId/languages` | ✅ | Add language |
| PUT | `/api/v1/users/:userId/languages/:id` | ✅ | Edit language |
| DELETE | `/api/v1/users/:userId/languages/:id` | ✅ | Delete language |
| POST | `/api/v1/users/:userId/recommendations` | ✅ | Write recommendation |
| PUT | `/api/v1/users/:userId/recommendations/:id` | ✅ | Edit recommendation |
| DELETE | `/api/v1/users/:userId/recommendations/:id` | ✅ | Delete recommendation |
| POST | `/api/v1/users/:userId/featured` | ✅ | Add featured item |
| PUT | `/api/v1/users/:userId/featured/:id` | ✅ | Edit featured item |
| DELETE | `/api/v1/users/:userId/featured/:id` | ✅ | Delete featured item |

**Subtotal: 46 endpoints**

---

## 8. Domain 3 — Connections & Network

### Requirements
- Send / accept / decline / withdraw invites
- Invite manager (received + sent)
- **Settings**: who can connect, what invitations to receive
- Block / unblock
- Suggestion categories: Same Company, Same School, Same Industry, Mutual Connections, Same Location, Colleagues, Recently Joined, PYMK
- 1st / 2nd / 3rd degree calculation

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| POST | `/api/v1/connections/request` | ✅ | Send invite |
| POST | `/api/v1/connections/request/:id/accept` | ✅ | Accept invite |
| DELETE | `/api/v1/connections/request/:id` | ✅ | Withdraw / decline |
| DELETE | `/api/v1/connections/:userId` | ✅ | Disconnect |
| GET | `/api/v1/connections` | ✅ | My connections |
| GET | `/api/v1/connections/received` | ✅ | Received invites |
| GET | `/api/v1/connections/sent` | ✅ | Sent invites |
| GET | `/api/v1/connections/suggestions` | ✅ | All PYMK |
| GET | `/api/v1/connections/suggestions/:category` | ✅ | By category (same-company etc.) |
| PUT | `/api/v1/connections/settings` | ✅ | Who can connect / what to receive |
| GET | `/api/v1/connections/settings` | ✅ | Get invite settings |
| POST | `/api/v1/connections/suggestions/:userId/dismiss` | ✅ | Dismiss suggestion |
| POST | `/api/v1/follow/:userId` | ✅ | Follow |
| DELETE | `/api/v1/follow/:userId` | ✅ | Unfollow |
| GET | `/api/v1/follow/followers` | ✅ | My followers |
| GET | `/api/v1/follow/following` | ✅ | Who I follow |
| POST | `/api/v1/users/:userId/block` | ✅ | Block |
| DELETE | `/api/v1/users/:userId/block` | ✅ | Unblock |
| GET | `/api/v1/users/:userId/degree` | ✅ | Connection degree |

**Subtotal: 19 endpoints**

---

## 9. Domain 4 — Feed / Posts & Media

### Post Types
- Text, Image (multi), Video, Document, Link (with preview), Poll, Article, Template/Celebration GIF (New Position, Work Anniversary, Promotion, etc.)

### Interactions
- **React**: Like, Celebrate, Support, Love, Insightful, Funny
- **Comment** + nested replies
- **Repost** — direct repost OR repost with thoughts
- **Share** — share with message to connections / group / company
- **Send** — send post as DM
- **Save** — to saved collections

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/feed` | ✅ | Ranked feed |
| POST | `/api/v1/posts` | ✅ | Create post (any type) |
| GET | `/api/v1/posts/:id` | ✅ | Get post |
| PUT | `/api/v1/posts/:id` | ✅ | Edit post |
| DELETE | `/api/v1/posts/:id` | ✅ | Delete post |
| POST | `/api/v1/posts/:id/react` | ✅ | Add reaction |
| DELETE | `/api/v1/posts/:id/react` | ✅ | Remove reaction |
| GET | `/api/v1/posts/:id/reactions` | ✅ | List reactions |
| POST | `/api/v1/posts/:id/comments` | ✅ | Add comment |
| GET | `/api/v1/posts/:id/comments` | ✅ | List comments |
| PUT | `/api/v1/comments/:id` | ✅ | Edit comment |
| DELETE | `/api/v1/comments/:id` | ✅ | Delete comment |
| POST | `/api/v1/comments/:id/react` | ✅ | React to comment |
| POST | `/api/v1/comments/:id/replies` | ✅ | Nested reply |
| POST | `/api/v1/posts/:id/repost` | ✅ | Repost (direct or with thoughts) |
| POST | `/api/v1/posts/:id/share` | ✅ | Share with message |
| POST | `/api/v1/posts/:id/send` | ✅ | Send post as DM |
| POST | `/api/v1/posts/:id/pin` | ✅ | Pin to profile |
| POST | `/api/v1/posts/:id/save` | ✅ | Save post |
| DELETE | `/api/v1/posts/:id/save` | ✅ | Unsave post |
| GET | `/api/v1/posts/saved` | ✅ | My saved posts |
| GET | `/api/v1/posts/saved/collections` | ✅ | My save collections |
| POST | `/api/v1/posts/:id/report` | ✅ | Report post |
| POST | `/api/v1/articles` | ✅ | Publish article |
| PUT | `/api/v1/articles/:id` | ✅ | Edit article |
| GET | `/api/v1/articles/:id` | ✅ | Read article |
| POST | `/api/v1/posts/draft` | ✅ | Save draft |
| GET | `/api/v1/posts/drafts` | ✅ | My drafts |
| GET | `/api/v1/hashtags/:tag/posts` | ✅ | Posts by hashtag |
| POST | `/api/v1/hashtags/:tag/follow` | ✅ | Follow hashtag |
| GET | `/api/v1/posts/templates` | ✅ | Available celebration templates |

**Subtotal: 31 endpoints**

---

## 10. Domain 5 — Messaging

### Requirements
- 1:1 DM, group chat, message reactions, read receipts, typing indicator
- Media + GIF + voice note attachments
- InMail (Premium — message non-connections, credits tracked)
- Archive / mute / delete conversations

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/conversations` | ✅ | List conversations |
| POST | `/api/v1/conversations` | ✅ | Start conversation |
| GET | `/api/v1/conversations/:id/messages` | ✅ | Get messages |
| POST | `/api/v1/conversations/:id/messages` | ✅ | Send message |
| PUT | `/api/v1/messages/:id` | ✅ | Edit message |
| DELETE | `/api/v1/messages/:id` | ✅ | Delete message |
| POST | `/api/v1/messages/:id/react` | ✅ | React to message |
| PUT | `/api/v1/conversations/:id/read` | ✅ | Mark read |
| PUT | `/api/v1/conversations/:id/mute` | ✅ | Mute |
| PUT | `/api/v1/conversations/:id/archive` | ✅ | Archive |
| DELETE | `/api/v1/conversations/:id` | ✅ | Delete |
| POST | `/api/v1/conversations/group` | ✅ | Create group chat |
| POST | `/api/v1/conversations/:id/members` | ✅ | Add to group |
| DELETE | `/api/v1/conversations/:id/members/:userId` | ✅ | Remove from group |
| POST | `/api/v1/inmail` | Premium | Send InMail |
| GET | `/api/v1/inmail/credits` | Premium | InMail credit balance |
| WS | `/ws/conversations/:id` | ✅ | Real-time + typing |

**Subtotal: 17 endpoints**

---

## 11. Domain 6 — Notifications

### Notification Types
Connection, Reaction, Comment, Mention, JobAlert, ProfileView, Birthday, CompanyUpdate, EndorsedSkill, RecommendationRequest, PostShared, GroupInvite, PremiumOffer

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/notifications` | ✅ | List (paginated) |
| PUT | `/api/v1/notifications/:id/read` | ✅ | Mark read |
| PUT | `/api/v1/notifications/read-all` | ✅ | Mark all read |
| DELETE | `/api/v1/notifications/:id` | ✅ | Delete |
| GET | `/api/v1/notifications/preferences` | ✅ | Get per-type prefs |
| PUT | `/api/v1/notifications/preferences` | ✅ | Update prefs |
| POST | `/api/v1/notifications/push-token` | ✅ | Register push token |
| WS | `/ws/notifications` | ✅ | Real-time |

**Subtotal: 8 endpoints**

---

## 12. Domain 7 — Jobs

### Job Categories & Filters
- Job Type: Full-time, Part-time, Contract, Temporary, Internship, Volunteer
- Workplace: On-site, Hybrid, Remote
- Experience Level: Internship, Entry, Associate, Mid-Senior, Director, Executive
- Industry + Function + Skills

### Personalized Suggestions
- Self-hosted/local recommendation algorithm (e.g., ML.NET or custom collaborative filtering/heuristics) based on: current role, skills, search history, applications, followed companies, network activity

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/jobs` | ✅ | Search + filter |
| POST | `/api/v1/jobs` | ✅ | Post job |
| GET | `/api/v1/jobs/:id` | ✅ | Job detail |
| PUT | `/api/v1/jobs/:id` | ✅ | Edit job |
| DELETE | `/api/v1/jobs/:id` | ✅ | Close job |
| POST | `/api/v1/jobs/:id/apply` | ✅ | Apply |
| GET | `/api/v1/jobs/:id/applicants` | ✅ | List applicants (employer) |
| PUT | `/api/v1/jobs/:id/applicants/:appId/stage` | ✅ | Move ATS stage |
| POST | `/api/v1/jobs/:id/save` | ✅ | Save job |
| DELETE | `/api/v1/jobs/:id/save` | ✅ | Unsave |
| GET | `/api/v1/jobs/saved` | ✅ | My saved jobs |
| GET | `/api/v1/jobs/recommended` | ✅ | ML recommendations |
| GET | `/api/v1/jobs/recommended/categories` | ✅ | Recs by category |
| GET | `/api/v1/jobs/applied` | ✅ | My applications |
| POST | `/api/v1/job-alerts` | ✅ | Create alert |
| GET | `/api/v1/job-alerts` | ✅ | List alerts |
| PUT | `/api/v1/job-alerts/:id` | ✅ | Edit alert |
| DELETE | `/api/v1/job-alerts/:id` | ✅ | Delete alert |
| GET | `/api/v1/jobs/categories` | ✅ | Industry / function list |

**Subtotal: 19 endpoints**

---

## 13. Domain 8 — Companies

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| POST | `/api/v1/companies` | ✅ | Create page |
| GET | `/api/v1/companies/:id` | ✅ | Company profile |
| PUT | `/api/v1/companies/:id` | ✅ | Edit page |
| POST | `/api/v1/companies/:id/follow` | ✅ | Follow |
| DELETE | `/api/v1/companies/:id/follow` | ✅ | Unfollow |
| GET | `/api/v1/companies/:id/followers` | ✅ | Follower list |
| GET | `/api/v1/companies/:id/employees` | ✅ | Employee list |
| POST | `/api/v1/companies/:id/admins` | ✅ | Add admin |
| DELETE | `/api/v1/companies/:id/admins/:userId` | ✅ | Remove admin |
| POST | `/api/v1/companies/:id/posts` | ✅ | Post update |
| GET | `/api/v1/companies/:id/posts` | ✅ | Company feed |
| GET | `/api/v1/companies/:id/analytics` | ✅ | Page analytics |
| POST | `/api/v1/companies/:id/products` | ✅ | Add product |
| PUT | `/api/v1/companies/:id/products/:prodId` | ✅ | Edit product |
| DELETE | `/api/v1/companies/:id/products/:prodId` | ✅ | Delete product |
| GET | `/api/v1/companies/:id/jobs` | ✅ | Company jobs |

**Subtotal: 16 endpoints**

---

## 14. Domain 9 — Groups

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| POST | `/api/v1/groups` | ✅ | Create group |
| GET | `/api/v1/groups/:id` | ✅ | Group profile |
| PUT | `/api/v1/groups/:id` | ✅ | Edit group |
| DELETE | `/api/v1/groups/:id` | ✅ | Delete group |
| POST | `/api/v1/groups/:id/join` | ✅ | Join / request |
| DELETE | `/api/v1/groups/:id/leave` | ✅ | Leave |
| GET | `/api/v1/groups/:id/members` | ✅ | Member list |
| PUT | `/api/v1/groups/:id/members/:userId/role` | ✅ | Change role |
| DELETE | `/api/v1/groups/:id/members/:userId` | ✅ | Remove member |
| GET | `/api/v1/groups/:id/posts` | ✅ | Group feed |
| POST | `/api/v1/groups/:id/posts` | ✅ | Post to group |
| GET | `/api/v1/groups/my` | ✅ | My groups |
| GET | `/api/v1/groups/suggested` | ✅ | Suggested groups |
| POST | `/api/v1/groups/:id/invite` | ✅ | Invite connection |

**Subtotal: 14 endpoints**

---

## 15. Domain 10 — Puzzle Games

### Games
- **Wordle-variant** (LinkedIn flavour)
- **Crossword** (mini daily)
- **Tango** — sun/moon logic grid
- **Queens** — non-attacking queens
- **Pinpoint** — word category guessing
- **Zip** — number path puzzle

Free users: limited game access. Premium: all games + hints.

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/games` | ✅ | Available games list |
| GET | `/api/v1/games/:type/daily` | ✅ | Today's puzzle |
| POST | `/api/v1/games/:type/daily/attempt` | ✅ | Submit attempt / guess |
| GET | `/api/v1/games/:type/history` | ✅ | My past results |
| GET | `/api/v1/games/:type/streak` | ✅ | My streak |
| GET | `/api/v1/games/leaderboard` | ✅ | Friends leaderboard |
| POST | `/api/v1/games/:type/daily/share` | ✅ | Generate shareable result |

**Subtotal: 7 endpoints**

---

## 16. Domain 11 — Search

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/search` | ✅ | Unified `?q=&type=` |
| GET | `/api/v1/search/people` | ✅ | People + filters |
| GET | `/api/v1/search/jobs` | ✅ | Jobs + filters |
| GET | `/api/v1/search/companies` | ✅ | Companies + filters |
| GET | `/api/v1/search/posts` | ✅ | Posts + articles |
| GET | `/api/v1/search/groups` | ✅ | Groups |
| GET | `/api/v1/search/typeahead` | ✅ | Autocomplete |
| GET | `/api/v1/search/recent` | ✅ | Recent searches |
| DELETE | `/api/v1/search/recent` | ✅ | Clear history |
| POST | `/api/v1/search/saved` | ✅ | Save search |
| GET | `/api/v1/search/saved` | ✅ | List saved searches |
| DELETE | `/api/v1/search/saved/:id` | ✅ | Delete saved search |

**Subtotal: 12 endpoints**

---

## 17. Domain 12 — Settings

### Setting Categories
1. **Privacy** — profile visibility, who can connect, active status, profile data
2. **Notifications** — per-type in-app/email/push toggles
3. **Job Seeking** — open to work config, recruiter visibility, Easy Apply
4. **Advertising** — personalized ads, interest-based ads, third-party
5. **Data & Privacy** — data download, social activity visibility, mentions
6. **Account** — language, theme, 2FA, trusted devices, deactivate/delete

### API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/settings` | ✅ | All settings |
| PUT | `/api/v1/settings/privacy` | ✅ | Update privacy |
| PUT | `/api/v1/settings/notifications` | ✅ | Update notification settings |
| PUT | `/api/v1/settings/job-seeking` | ✅ | Update job seeking |
| PUT | `/api/v1/settings/advertising` | ✅ | Update ad preferences |
| PUT | `/api/v1/settings/data-privacy` | ✅ | Update data settings |
| PUT | `/api/v1/settings/account` | ✅ | Update account (lang, theme) |
| POST | `/api/v1/settings/data-download` | ✅ | Request data archive |
| DELETE | `/api/v1/settings/account` | ✅ | Deactivate account |
| DELETE | `/api/v1/settings/account/permanent` | ✅ | Permanently delete account |

**Subtotal: 10 endpoints**

---

## 18. Analytics

| Method | Endpoint | Auth | Description |
|--------|----------|:----:|-------------|
| GET | `/api/v1/analytics/profile` | ✅ | Impressions, views, search appearances |
| GET | `/api/v1/analytics/posts` | ✅ | Per-post reach, impressions |
| GET | `/api/v1/analytics/followers` | ✅ | Follower demographics |
| GET | `/api/v1/analytics/company/:id` | ✅ | Company page analytics |

**Subtotal: 4 endpoints**

---

## 19. Endpoint Count Summary

| Domain | Endpoints |
|--------|:---------:|
| Premium | 7 |
| Auth | 10 |
| Profile | 46 |
| Connections & Network | 19 |
| Feed / Posts & Media | 31 |
| Messaging | 17 |
| Notifications | 8 |
| Jobs | 19 |
| Companies | 16 |
| Groups | 14 |
| Puzzle Games | 7 |
| Search | 12 |
| Settings | 10 |
| Analytics | 4 |
| **TOTAL** | **220** |

---

## Enums Reference

```csharp
AccountTier:        Free | Premium | PremiumCareer | PremiumBusiness | SalesNavigator | Recruiter
PostType:           Text | Image | Video | Document | Link | Poll | Article | Template | Celebration
TemplateType:       NewPosition | WorkAnniversary | StartedEducation | Promotion | OpenToWork
ReactionType:       Like | Celebrate | Support | Love | Insightful | Funny
RepostType:         DirectRepost | RepostWithThoughts
NotificationType:   Connection | Reaction | Comment | Mention | JobAlert | ProfileView | ...
JobType:            FullTime | PartTime | Contract | Temporary | Internship | Volunteer
WorkplaceType:      OnSite | Hybrid | Remote
ExperienceLevel:    Internship | EntryLevel | Associate | MidSenior | Director | Executive
ApplicationStage:   Applied | Viewed | InReview | PhoneScreen | Interview | Offer | Rejected
SuggestionCategory: PeopleYouMayKnow | SameCompany | SameSchool | SameIndustry | MutualConnections | ...
GroupRole:          Member | Moderator | Admin | Owner
GroupVisibility:    Public | Private | Hidden
SavedItemType:      Post | Article | Job | Course
GameType:           Wordle | Trivia | Crossword | Tango | Queens | Pinpoint | Zip
```

---

*LinkedIn Clone — v1.0 | ASP.NET Core 8 | 220 endpoints | 60 models | 12 domains*
