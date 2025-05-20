import React, { useState } from 'react';
import './MessageInput.css';

const MessageInput = ({ sendMessage }) => {
  const [message, setMessage] = useState('');

  const handleSend = (e) => {
    e.preventDefault();
    if (message.trim()) {
      sendMessage(message);
      setMessage('');
    }
  };

  return (
    <form className="message-form" onSubmit={handleSend}>
      <input 
        type="text" 
        value={message} 
        onChange={(e) => setMessage(e.target.value)} 
        placeholder="Введіть ваше повідомлення..."
        className="message-input"
      />
      <button 
        type="submit" 
        disabled={!message.trim()} 
        className="send-button"
      >
        Надіслати
      </button>
    </form>
  );
};

export default MessageInput;