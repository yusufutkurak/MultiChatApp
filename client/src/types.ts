export interface ChatSession {
    id: string; 
    title: string;
    createdAt: string;
}

export interface ChatMessage {
    id: string;
    sessionId: string;
    content: string;
    sender: string; 
    model?: string;
    createdAt: string;
}