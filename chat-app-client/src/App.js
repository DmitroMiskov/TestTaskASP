import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import ChatWindow from './components/ChatWindow';
import MessageInput from './components/MessageInput';
import './App.css';

const App = () => {
  const [connection, setConnection] = useState(null);
  const [messages, setMessages] = useState([]);
  const [username, setUsername] = useState('');
  const [isUsernameSet, setIsUsernameSet] = useState(false);
  const [connectionStatus, setConnectionStatus] = useState('Відключено');
  const latestMessages = useRef(null);
  
  latestMessages.current = messages;

  useEffect(() => {
    // Встановлюємо з'єднання з сервером SignalR при першому завантаженні сторінки
    const connect = async () => {
      try {
        console.log('Спроба підключення до SignalR Hub...');
        
        // Використовуємо повний URL для з'єднання
        const hubUrl = 'https://localhost:7161/chatHub'; // Змініть на ваш порт бекенду
        console.log('Адреса хаба:', hubUrl);
        
        const connection = new HubConnectionBuilder()
          .withUrl(hubUrl)
          .configureLogging(LogLevel.Debug) // Підвищити рівень логування
          .withAutomaticReconnect([0, 2000, 5000, 10000, null]) // Стратегія переп'єднання
          .build();

        // Логування подій з'єднання
        connection.onreconnecting(error => {
          console.log('Втрата зєднання. Намагаємося перепєднатися...', error);
          setConnectionStatus('Перепєднання...');
        });

        connection.onreconnected(connectionId => {
          console.log('Перепєднано!', connectionId);
          setConnectionStatus('Підключено');
          // Оновлюємо історію повідомлень після переп'єднання
          connection.invoke('GetMessageHistory').catch(err => console.error('Не вдалося отримати історію:', err));
        });

        connection.onclose(error => {
          console.log('Зєднання закрито', error);
          setConnectionStatus('Відключено');
        });

        // Обробник отримання нового повідомлення
        connection.on('ReceiveMessage', (message) => {
          console.log('Отримано нове повідомлення:', message);
          const updatedMessages = [...latestMessages.current, message];
          setMessages(updatedMessages);
        });

        // Обробник отримання історії повідомлень
        connection.on('ReceiveMessageHistory', (messageHistory) => {
          console.log('Отримано історію повідомлень:', messageHistory);
          setMessages(messageHistory);
        });

        // Спроба підключення
        console.log('Починаємо зєднання...');
        await connection.start();
        console.log('Зєднання встановлено!', connection.connectionId);
        setConnectionStatus('Підключено');
        setConnection(connection);

        // Запитуємо історію повідомлень після успішного підключення
        console.log('Запитуємо історію повідомлень...');
        await connection.invoke('GetMessageHistory');
        console.log('Історію повідомлень отримано');
      } catch (e) {
        console.error('Помилка підключення:', e);
        setConnectionStatus(`Помилка: ${e.message}`);
      }
    };

    // З'єднуємось, якщо ще не з'єднались
    if (!connection) {
      connect();
    }

    // Очищення при розмонтуванні компонента
    return () => {
      if (connection) {
        console.log('Завершення зєднання...');
        connection.stop();
      }
    };
  }, [connection]);

  // Обробник надсилання повідомлення
  const sendMessage = async (messageText) => {
    if (connection && messageText && username) {
      try {
        console.log(`Надсилаємо повідомлення від ${username}: ${messageText}`);
        await connection.invoke('SendMessage', username, messageText);
        console.log('Повідомлення успішно надіслано');
      } catch (e) {
        console.error('Помилка надсилання повідомлення:', e);
        alert(`Не вдалося надіслати повідомлення: ${e.message}`);
      }
    } else {
      console.warn('Не можу надіслати повідомлення: зєднання відсутнє або дані неповні');
      if (!connection) console.warn('- Зєднання відсутнє');
      if (!messageText) console.warn('- Текст повідомлення порожній');
      if (!username) console.warn('- Імя користувача порожнє');
    }
  };

  // Функція для встановлення імені користувача
  const setUsernameFn = (name) => {
    console.log(`Ім'я користувача встановлено: ${name}`);
    setUsername(name);
    setIsUsernameSet(true);
    // Зберігаємо ім'я в локальному сховищі для майбутніх сесій
    localStorage.setItem('chatUsername', name);
  };

  // Перевіряємо, чи є збережене ім'я при завантаженні
  useEffect(() => {
    const savedUsername = localStorage.getItem('chatUsername');
    if (savedUsername) {
      console.log(`Знайдено збережене ім'я: ${savedUsername}`);
      setUsername(savedUsername);
      setIsUsernameSet(true);
    }
  }, []);

  // Якщо користувач ще не ввів ім'я, показуємо форму входу
  if (!isUsernameSet) {
    return (
      <div className="container">
        <div className="login-container">
          <h2>Введіть ваше ім'я, щоб приєднатися до чату</h2>
          <input
            type="text"
            placeholder="Ваше ім'я"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="username-input"
          />
          <button 
            onClick={() => setUsernameFn(username)}
            disabled={!username.trim()}
            className="join-button"
          >
            Приєднатися до чату
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <h1>Чат в реальному часі з аналізом тональності</h1>
      <div className="user-info">
        Увійшли як: <strong>{username}</strong> | Статус: <span className={`status-${connectionStatus.toLowerCase()}`}>{connectionStatus}</span>
      </div>
      <ChatWindow messages={messages} currentUser={username} />
      <MessageInput sendMessage={sendMessage} />
      <div className="connection-status">
        <small>Стан з'єднання: {connectionStatus}</small>
        {connectionStatus !== 'Підключено' && (
          <button 
            onClick={() => {
              setConnection(null); // Спробувати переп'єднатися
            }}
            className="reconnect-button"
          >
            Переп'єднатися
          </button>
        )}
      </div>
    </div>
  );
};

export default App;