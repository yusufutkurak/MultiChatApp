import { useEffect, useState, useRef } from 'react';
import './App.css';
import { chatApi } from './api';
import type { ChatSession, ChatMessage } from './types';
import ReactMarkdown from 'react-markdown';

// İkonlar
import { FiPlus, FiMessageSquare, FiTrash2, FiSend, FiSidebar, FiCpu, FiChevronDown, FiCheck } from 'react-icons/fi';
import { RiRobot2Line } from 'react-icons/ri';

// Model Listesi
const MODELS = [
  { id: "gemini-2.5-flash", name: "Gemini 2.5 Flash" },
  { id: "gemini-2.5-flash-lite", name: "Gemini 2.5 Flash Lite" },
  { id: "gpt-5-mini", name: "GPT 5 Mini" },
  { id: "gpt-4.1", name: "GPT 4.1" },
];

function App() {
  const [sessions, setSessions] = useState<ChatSession[]>([]);
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState('');
  
  const [selectedModel, setSelectedModel] = useState("gemini-3-flash-preview");
  const [isModelDropdownOpen, setIsModelDropdownOpen] = useState(false);

  const [isLoading, setIsLoading] = useState(false);
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);

  const messagesEndRef = useRef<HTMLDivElement>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => { loadSessions(); }, []);

  useEffect(() => {
    if (currentSessionId) loadMessages(currentSessionId);
    else setMessages([]);
  }, [currentSessionId]);

  useEffect(() => {
    scrollToBottom();
  }, [messages, isLoading]);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsModelDropdownOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const loadSessions = async () => {
    try {
      const data = await chatApi.getAllSessions();
      setSessions(data);
    } catch (error) { console.error(error); }
  };

  const loadMessages = async (sessionId: string) => {
    try {
      const data = await chatApi.getMessages(sessionId);
      setMessages(data);
    } catch (error) { console.error(error); }
  };

  // --- GÜNCELLENEN KISIM: HEMEN OLUŞTURMA ---
  const handleNewChat = async () => {
    try {
      // 1. Backend'e git ve boş bir sohbet oluştur
      const newSession = await chatApi.createSession();
      
      // 2. Listeye ekle (En başa)
      setSessions([newSession, ...sessions]);
      
      // 3. O sohbeti seçili yap
      setCurrentSessionId(newSession.id);
      
      // 4. Ekranı temizle
      setMessages([]);
      setInput('');
      
      // Mobildeysek sidebar'ı kapatabiliriz (İsteğe bağlı)
      // setIsSidebarOpen(false); 
    } catch (error) {
      console.error("Yeni sohbet oluşturulamadı:", error);
    }
  };

  const handleDeleteSession = async (e: React.MouseEvent, sessionId: string) => {
    e.stopPropagation();
    if(!confirm("Are you sure you want to delete this chat?")) return;
    try {
      await chatApi.deleteSession(sessionId);
      setSessions(sessions.filter(s => s.id !== sessionId));
      if (currentSessionId === sessionId) {
        setCurrentSessionId(null);
        setMessages([]);
      }
    } catch (error) { console.error(error); }
  };

  const handleDeleteMessage = async (messageId: string) => {
    try {
      await chatApi.deleteMessage(messageId);
      setMessages(prev => prev.filter(m => m.id !== messageId));
    } catch (error) {
      console.error("Mesaj silinemedi", error);
    }
  };

  const handleSendMessage = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!input.trim()) return;

    const userContent = input;
    setInput('');
    setIsLoading(true);

    // Eğer bir şekilde ID yoksa (New Chat'e basmadan yazdıysa) oluştur
    let activeSessionId = currentSessionId;
    if (!activeSessionId) {
      try {
        const newSession = await chatApi.createSession();
        setSessions(prev => [newSession, ...prev]);
        setCurrentSessionId(newSession.id);
        activeSessionId = newSession.id;
      } catch (error) {
        console.error("Session creation failed", error);
        setIsLoading(false);
        return;
      }
    }

    const tempUserMsg: ChatMessage = {
      id: Date.now().toString(),
      sessionId: activeSessionId!,
      content: userContent,
      sender: 'User',
      createdAt: new Date().toISOString()
    };
    setMessages(prev => [...prev, tempUserMsg]);

    try {
      await chatApi.sendMessage(activeSessionId!, userContent, selectedModel);
      await loadMessages(activeSessionId!);
    } catch (error) {
      alert("Error sending message.");
    } finally {
      setIsLoading(false);
    }
  };

  const currentModelName = MODELS.find(m => m.id === selectedModel)?.name || selectedModel;

  return (
    <div className="app-container">
      
      {/* --- SIDEBAR --- */}
      <div className={`sidebar ${isSidebarOpen ? '' : 'closed'}`}>
        <div className="sidebar-header"></div>
        
        <button className="new-chat-btn" onClick={handleNewChat}>
          <FiPlus size={16} /> New Chat
        </button>
        
        <div className="session-list">
          {sessions.map(session => (
            <div 
              key={session.id} 
              className={`session-item ${currentSessionId === session.id ? 'active' : ''}`}
              onClick={() => setCurrentSessionId(session.id)}
            >
              <div style={{display:'flex', alignItems:'center', gap:'8px', overflow:'hidden'}}>
                <FiMessageSquare size={14} />
                <span style={{textOverflow:'ellipsis', overflow:'hidden'}}>{session.title}</span>
              </div>
              {currentSessionId === session.id && (
                <button 
                  className="icon-btn"
                  onClick={(e) => handleDeleteSession(e, session.id)}
                >
                  <FiTrash2 size={14} />
                </button>
              )}
            </div>
          ))}
        </div>
      </div>

      {/* --- CHAT AREA --- */}
      <div className="chat-area">
        
        {/* HEADER BAR */}
        <div className="header-bar">
          <button 
            className="icon-btn" 
            onClick={() => setIsSidebarOpen(!isSidebarOpen)}
            title="Toggle Sidebar"
          >
            <FiSidebar size={20} />
          </button>

          {/* CUSTOM MODEL DROPDOWN */}
          <div className="custom-select-container" ref={dropdownRef}>
            <button 
              className={`custom-select-trigger ${isModelDropdownOpen ? 'active' : ''}`}
              onClick={() => setIsModelDropdownOpen(!isModelDropdownOpen)}
            >
              {currentModelName}
              <FiChevronDown size={14} style={{ opacity: 0.5 }} />
            </button>

            {isModelDropdownOpen && (
              <div className="custom-options-list">
                {MODELS.map((model) => (
                  <div 
                    key={model.id}
                    className={`custom-option ${selectedModel === model.id ? 'selected' : ''}`}
                    onClick={() => {
                      setSelectedModel(model.id);
                      setIsModelDropdownOpen(false);
                    }}
                  >
                    {model.name}
                    {selectedModel === model.id && <FiCheck size={14} />}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* MESSAGES & CONTENT */}
        <div className="messages-container">
            {/* Eğer Mesaj Yoksa "Hoşgeldin" Göster */}
            {(!currentSessionId || messages.length === 0) ? (
                <div style={{display:'flex', flexDirection:'column', alignItems:'center', justifyContent:'center', height:'100%', color:'#52525b', gap:'20px'}}>
                    <RiRobot2Line size={48} style={{opacity:0.2}} />
                    <h3>How can I help you today?</h3>
                </div>
            ) : (
                <>
                    {messages.map((msg) => {
                      const isUser = msg.sender === 'User';
                      return (
                        <div key={msg.id} className={`message-wrapper ${isUser ? 'user' : 'gemini'}`}>
                          
                          {!isUser && (
                            <div className="avatar">
                              {msg.model?.startsWith('gpt') ? <FiCpu size={18} /> : <RiRobot2Line size={18} />}
                            </div>
                          )}

                          <div className="message-content">
                            <div className="markdown-content">
                                <ReactMarkdown>{msg.content}</ReactMarkdown>
                            </div>
                          </div>

                          <div className="message-actions">
                            <button 
                              className="msg-delete-btn" 
                              onClick={() => handleDeleteMessage(msg.id)}
                              title="Delete message"
                            >
                              <FiTrash2 size={14} />
                            </button>
                          </div>

                        </div>
                      );
                    })}

                    {isLoading && (
                    <div className="message-wrapper gemini">
                        <div className="avatar"><RiRobot2Line size={18} /></div>
                        <div className="message-content">
                        <div className="typing-indicator">
                            <div className="typing-dot"></div>
                            <div className="typing-dot"></div>
                            <div className="typing-dot"></div>
                        </div>
                        </div>
                    </div>
                    )}
                    <div ref={messagesEndRef} />
                </>
            )}
        </div>

        {/* INPUT AREA */}
        <div className="input-area">
          <div className="input-container">
            <form onSubmit={handleSendMessage} style={{display:'flex', width:'100%', alignItems:'center'}}>
              <input 
                className="chat-input"
                placeholder="Type a message..."
                value={input}
                onChange={(e) => setInput(e.target.value)}
                disabled={isLoading}
                autoFocus
              />
              <button type="submit" className="send-btn" disabled={!input.trim() || isLoading}>
                <FiSend size={18} />
              </button>
            </form>
          </div>
        </div>

      </div>
    </div>
  );
}

export default App;