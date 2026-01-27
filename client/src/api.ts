import axios from 'axios';
import type { ChatSession, ChatMessage } from './types';
const API_URL = 'http://localhost:7050/api/Chat';

export const chatApi = {
    getAllSessions: async () => {
        const response = await axios.get<ChatSession[]>(`${API_URL}/sessions`);
        return response.data;
    },

    createSession: async () => {
        const response = await axios.post<ChatSession>(`${API_URL}/sessions`);
        return response.data;
    },

    getMessages: async (sessionId: string) => {
        const response = await axios.get<ChatMessage[]>(`${API_URL}/sessions/${sessionId}/messages`);
        return response.data;
    },

    sendMessage: async (sessionId: string, content: string, model: string) => {
        const response = await axios.post(`${API_URL}/sessions/${sessionId}/messages`, { content, model });
        return response.data;
    },
    
    deleteSession: async (sessionId: string) => {
        await axios.delete(`${API_URL}/sessions/${sessionId}`);
    },

    deleteMessage: async (messageId: string) => {
        await axios.delete(`${API_URL}/messages/${messageId}`);
    }
};


