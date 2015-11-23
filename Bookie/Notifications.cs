using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Bookie
{
    public static class Notifications
    {
        public static ToastNotification ShowToast(string line1, string line2)
        {
            var template = new StringBuilder();
            template.Append("<toast><visual version='2'><binding template='ToastText02'>");
            template.AppendFormat("<text id='2'>{0}</text>", line1);
            template.AppendFormat("<text id='1'>{0}</text>", line2);
            template.Append("</binding></visual></toast>");
            var xml = new XmlDocument();
            xml.LoadXml(template.ToString());
            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
        }
    }
}