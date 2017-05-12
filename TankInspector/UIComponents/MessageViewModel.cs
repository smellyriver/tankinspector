using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using Smellyriver.Utilities;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.UIComponents
{
    public class MessageViewModel : NotificationObject
    {

        private string _title;
        public string Title
        {
            get => _title;
	        private set
            {
                _title = value;
                this.RaisePropertyChanged(() => this.Title);
            }
        }


        private FlowDocument _body;
        public FlowDocument Body
        {
            get => _body;
	        private set
            {
                _body = value;
                this.RaisePropertyChanged(() => this.Body);
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
	        set
            {
                _message = value;
                this.RaisePropertyChanged(() => this.Message);
                this.ParseMessage(_message);
            }
        }

        private DateTime _time;
        public DateTime Time
        {
            get => _time;
	        set
            {
                _time = value;
                this.RaisePropertyChanged(() => this.Time);
            }
        }

        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
	        set
            {
                _isRead = value;
                this.RaisePropertyChanged(() => this.IsRead);
            }
        }


        public MessageViewModel()
        {

        }

        public MessageViewModel(string message, DateTime time)
        {
            this.Message = message;
            this.Time = time;
        }

        private void ParseMessage(string message)
        {
            var match = Regex.Match(message.Replace("\r\n", "<LineBreak/>").Replace("\n", "<LineBreak/>").Replace("\r", "<LineBreak/>"), @"(\@(?<title>.+?)\:)*(?<content>.+)", RegexOptions.Multiline);

            if (match.Success)
            {
                this.Title = match.Groups["title"].Value;
                var content = match.Groups["content"].Value;
                content = content.Replace("<p>", "<Paragraph>");
                content = content.Replace("</p>", "</Paragraph>");
                content = content.Replace("<b>", "<Bold>");
                content = content.Replace("</b>", "</Bold>");
                content = content.Replace("<i>", "<Italic>");
                content = content.Replace("</i>", "</Italic>");
                content = content.Replace("<br>", "<LineBreak/>");
                content = Regex.Replace(content, @"\<a\>(?<url>.+?)\<\/a\>", "<Hyperlink NavigateUri=\"${url}\">${url}</Hyperlink>");
                content = Regex.Replace(content, @"\<a\:(?<url>.+?)\>", "<Hyperlink NavigateUri=\"${url}\">");
                content = content.Replace("</a>", "</Hyperlink>");

                if (!content.StartsWith("<Paragraph>"))
                    content = $"<Paragraph>{content}</Paragraph>";

                content =
	                $"<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{content}</FlowDocument>";

                try
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                    {
                        this.Body = XamlReader.Load(stream) as FlowDocument;
                    }
                }
#if DEBUG
                catch (Exception ex)
#else
                catch (Exception)
#endif
                {
                    var document = new FlowDocument();
                    var paragraph = new Paragraph();
#if DEBUG
                    var run = new Run(ex.ToInformationString());
#else
                    var run = new Run(App.GetLocalizedString("InvalidMessage"));
#endif
                    paragraph.Inlines.Add(run);
                    document.Blocks.Add(paragraph);
                    this.Body = document;
                }
            }
        }
    }
}
