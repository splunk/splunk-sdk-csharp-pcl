namespace Splunk.Sdk
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    public class Message
    {
        internal Message(XElement message)
        {
            Contract.Requires(message != null);
            Contract.Requires(message.Name == "msg");

            string type = message.Attribute("type").Value;

            switch (message.Attribute("type").Value)
            {
                case "DEBUG":
                    this.Type = MessageType.Debug;
                    break;
                case "INFO":
                    this.Type = MessageType.Information;
                    break;
                case "WARN":
                    this.Type = MessageType.Warning;
                    break;
                case "ERROR":
                    this.Type = MessageType.Error;
                    break;
                default:
                    Contract.Requires(false, string.Format("Unrecognized message type: {0}", type));
                    break;
            }
            
            this.Text = message.Value;
        }

        public string Text
        { get; private set; }

        public MessageType Type
        { get; private set; }
    }
}
