namespace Cipso.Registry.Api.Domain;

public enum SummonsStatus
{
    Draft,
    Issued,
    Delivered,
    Acknowledged,
    Completed,
    Cancelled
}

public enum NotificationChannel
{
    Email,
    Sms,
    Portal
}

public enum NotificationStatus
{
    Queued,
    Delivered,
    Failed
}

public enum AppealStatus
{
    Submitted,
    InReview,
    Resolved,
    Rejected
}
