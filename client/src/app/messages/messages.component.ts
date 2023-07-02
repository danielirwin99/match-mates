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
  loading = false;

  ngOnInit(): void {
    this.loadMessages();
  }

  constructor(private messageService: MessageService) {}

  // LOADING THE MESSAGES
  loadMessages() {
    this.loading = true;
    this.messageService
      .getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe({
        next: (response) => {
          // Syncing the message to the response
          this.messages = response.result;
          // Syncing the pagination pages to the response
          this.pagination = response.pagination;
          this.loading = false;
        },
      });
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      // Finding the message that fits the parameter we are passing in (id)
      next: () =>
        this.messages?.splice(
          this.messages.findIndex((m) => m.id === id),
          1 // Deleting 1 message
        ),
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
