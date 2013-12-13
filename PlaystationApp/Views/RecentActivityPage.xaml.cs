using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Views
{
    public partial class RecentActivityPage : PhoneApplicationPage
    {
        public RecentActivityPage()
        {
            InitializeComponent();
        }

        private int StoryCount { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StoryCount = 0;
            RecentActivityEntity.Feed recentActivityFeedEntity = App.SelectedRecentActivityFeedEntity;
            ContentPanel.DataContext = App.SelectedRecentActivityFeedEntity;
            if (recentActivityFeedEntity.CondensedStories != null)
            {
                ActivityPageCount.Text = string.Format("1/{0}", recentActivityFeedEntity.CondensedStories.Count);
                SetDataContentCondensedStories(App.SelectedRecentActivityFeedEntity.StoryType,
                    App.SelectedRecentActivityFeedEntity.CondensedStories[StoryCount]);
                ActivityPageGrid.Visibility = Visibility.Visible;
            }
            else
            {
                SetDataContent(recentActivityFeedEntity.StoryType);
            }
        }

        private void SetDataContent(string storyType)
        {
            RecentActivityEntity.Feed recentActivityFeedEntity = App.SelectedRecentActivityFeedEntity;
            RecentActivityEntity.Target target;
            switch (storyType)
            {
                case "TROPHY":
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_IMAGE_URL"));
                    if (target != null)
                        MainImage.Source = (ImageSource) new ImageSourceConverter().ConvertFromString(target.Meta);
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_DETAIL"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta;
                    return;
                case "PLAYED_GAME":
                    MainImage.Source =
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(recentActivityFeedEntity.SmallImageUrl);
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n");
                    return;
                case "FRIENDED":
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("ONLINE_ID"));
                    if (target != null)
                    {
                        MainImage.Source = (ImageSource) new ImageSourceConverter().ConvertFromString(target.ImageUrl);
                        ActivityHeaderTextBlock.Text = target.Meta;
                        ActivityTextBlock.Text = string.Empty;
                    }
                    return;
                case "BROADCASTING":
                    MainImage.Source =
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(recentActivityFeedEntity.SmallImageUrl);
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = recentActivityFeedEntity.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n");
                    return;
                case "PROFILE_PIC":
                    MainImage.Source =
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(recentActivityFeedEntity.LargeImageUrl);
                    ActivityHeaderTextBlock.Text = recentActivityFeedEntity.Caption;
                    return;
                default:
                    return;
            }
        }

        private void SetDataContentCondensedStories(string storyType, RecentActivityEntity.CondensedStory condensedStory)
        {
            RecentActivityEntity.Feed recentActivityFeedEntity = App.SelectedRecentActivityFeedEntity;
            RecentActivityEntity.Target target;
            switch (storyType)
            {
                case "TROPHY":
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_IMAGE_URL"));
                    if (target != null)
                        MainImage.Source = (ImageSource) new ImageSourceConverter().ConvertFromString(target.Meta);
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_DETAIL"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta;
                    return;
                case "PLAYED_GAME":
                    MainImage.Source =
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(recentActivityFeedEntity.SmallImageUrl);
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br><br>", "\n\n");
                    return;
                case "FRIENDED":
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("ONLINE_ID"));
                    if (target != null)
                    {
                        MainImage.Source = (ImageSource) new ImageSourceConverter().ConvertFromString(target.ImageUrl);
                        ActivityHeaderTextBlock.Text = target.Meta;
                        ActivityTextBlock.Text = string.Empty;
                    }
                    return;
                case "BROADCASTING":
                    MainImage.Source =
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(condensedStory.SmallImageUrl);
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n");
                    return;
                default:
                    return;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask
            {
                Uri =
                    new Uri(
                        App.SelectedRecentActivityFeedEntity.ProductUrl)
            };
            task.Show();
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            StoryCount--;
            ContentPanel.DataContext = App.SelectedRecentActivityFeedEntity.CondensedStories[StoryCount];
            if (StoryCount == 0)
            {
                BackButton.IsEnabled = false;
            }
            ForwardButton.IsEnabled = true;
            SetDataContentCondensedStories(App.SelectedRecentActivityFeedEntity.StoryType,
                App.SelectedRecentActivityFeedEntity.CondensedStories[StoryCount]);
            ActivityPageCount.Text = string.Format("{0}/{1}", StoryCount + 1,
                App.SelectedRecentActivityFeedEntity.CondensedStories.Count);
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            StoryCount++;
            if (StoryCount >= App.SelectedRecentActivityFeedEntity.CondensedStories.Count - 1)
            {
                ForwardButton.IsEnabled = false;
            }
            BackButton.IsEnabled = true;
            SetDataContentCondensedStories(App.SelectedRecentActivityFeedEntity.StoryType,
                App.SelectedRecentActivityFeedEntity.CondensedStories[StoryCount]);
            ContentPanel.DataContext = App.SelectedRecentActivityFeedEntity.CondensedStories[StoryCount];
            ActivityPageCount.Text = string.Format("{0}/{1}", StoryCount + 1,
                App.SelectedRecentActivityFeedEntity.CondensedStories.Count);
        }
    }
}