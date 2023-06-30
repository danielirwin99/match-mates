import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination?: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;

  ngOnInit(): void {
    this.loadMessages()
  }

  constructor(private messageService: MessageService) {}

  // LOADING THE MESSAGES
  loadMessages() {
    this.messageService
      .getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe({
        next: (response) => {
          // Syncing the message to the response
          this.messages = response.result;
          // Syncing the pagination pages to the response
          this.pagination = response.pagination;
        },
      });
  }

  // What happens when you change the page
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      // Syncing the response to the page
      this.pageNumber = event.page;
      this.loadMessages;
    }
  }
}
