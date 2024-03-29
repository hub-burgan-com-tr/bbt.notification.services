﻿public class PostInstantCustomerPermissionRequest
{
    public bool showWithoutLogin { get; set; }
    public List<ReminderPost> reminders { get; set; }
}
public class ReminderPost
{
    public string reminderType { get; set; }
    public bool sms { get; set; }
    public bool email { get; set; }
    public bool mobileNotification { get; set; }
    public bool hasAmountRestriction { get; set; }
    public decimal amount { get; set; }
}

public class PostInstantCustomerPermissionRequestForLog
{
    public string identityNo { get; set; }
    public string modifiedBy { get; set; }
    public bool showWithoutLogin { get; set; }
    public List<ReminderPost> reminders { get; set; }
}