import { Component } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

interface ChatMessage {
  role: 'user' | 'bot';
  text?: string;
  products?: any[];
}

@Component({
  selector: 'app-chat-widget',
  imports: [CommonModule,FormsModule,RouterLink],
  templateUrl: './chat-widget.html',
  styleUrl: './chat-widget.css',
})
export class ChatWidget {
isOpen = false;
  userInput = '';
  isLoading = false;
  messages: ChatMessage[] = [
    { role: 'bot', text: 'Hi! Tell me what product you\'re looking for — I\'ll find the best match for you.' }
  ];

  constructor(private apiService: ApiService) {}

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  sendMessage() {
    const message = this.userInput.trim();
    if (!message || this.isLoading) return;

    // Add user message
    this.messages.push({ role: 'user', text: message });
    this.userInput = '';
    this.isLoading = true;

    this.apiService.chatSearch(message).subscribe({
      next: (response) => {
        const matches = response.matches || [];
        if (matches.length > 0) {
          this.messages.push({ role: 'bot', products: matches });
        } else {
          this.messages.push({
            role: 'bot',
            text: 'I couldn\'t find a matching product. I\'ve notified the admin to add it!'
          });
        }
        this.isLoading = false;
      },
      error: () => {
        this.messages.push({
          role: 'bot',
          text: 'Something went wrong. Please try again.'
        });
        this.isLoading = false;
      }
    });
  }

  onEnter(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.sendMessage();
    }
  }
}
