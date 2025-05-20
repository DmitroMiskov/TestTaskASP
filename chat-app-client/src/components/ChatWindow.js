import React from 'react';
import './ChatWindow.css';

class ChatWindow extends React.Component {
  constructor(props) {
    super(props);
    this.messagesEndRef = React.createRef();
  }

  componentDidUpdate() {
    this.scrollToBottom();
  }

  scrollToBottom() {
    if (this.messagesEndRef.current) {
      this.messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }

  getSentimentClass(message) {
    if (!message.sentimentScore && message.sentimentScore !== 0) return '';
    
    if (message.sentimentType === 'positive') return 'positive-sentiment';
    if (message.sentimentType === 'negative') return 'negative-sentiment';
    return 'neutral-sentiment';
  }

  getSentimentEmoji(message) {
    if (!message.sentimentScore && message.sentimentScore !== 0) return '';
    
    if (message.sentimentType === 'positive') return '😊';
    if (message.sentimentType === 'negative') return '😔';
    return '😐';
  }

  render() {
    const { messages, currentUser } = this.props;
    
    return (
      <div className="chat-window">
        <div className="message-list">
          {messages.map((message, index) => (
            <div 
              key={index} 
              className={`message ${message.username === currentUser ? 'my-message' : 'other-message'} ${this.getSentimentClass(message)}`}
            >
              <div className="message-header">
                <span className="username">{message.username}</span>
                <span className="timestamp">
                  {new Date(message.timestamp).toLocaleTimeString()}
                </span>
              </div>
              <div className="message-content">
                {message.content}
                {message.sentimentScore !== null && (
                  <span className="sentiment-indicator" title={`Sentiment Score: ${message.sentimentScore?.toFixed(2)}`}>
                    {this.getSentimentEmoji(message)}
                  </span>
                )}
              </div>
            </div>
          ))}
          <div ref={this.messagesEndRef} />
        </div>
      </div>
    );
  }
}

export default ChatWindow;