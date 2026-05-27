namespace H5YR.Core.Models.ViewModels
{
  public class CommunityStatsViewModel
  {
    public int TotalH5yrs { get; set; }
    public int TotalWidgetSubmissions { get; set; }
    public int TodayCount { get; set; }
    public List<ActivityDataPoint> ActivityTimeline { get; set; } = new();
    public List<ContributorDataPoint> TopContributors { get; set; } = new();
  }

  public class ActivityDataPoint
  {
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
  }

  public class ContributorDataPoint
  {
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = string.Empty;
    public int Count { get; set; }
  }
}
