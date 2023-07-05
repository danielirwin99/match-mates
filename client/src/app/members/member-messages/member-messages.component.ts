import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  // This component is a child of member-detailed.component
  @Input() username?: string;

  messageContent = '';

  constructor(public messageService: MessageService) {}

  ngOnInit(): void {}

  sendMessage() {
    // Checking if we have the username --> If we don't, stop the function
    if (!this.username) return;

    // We are returning a Promise (from sendMessage in message.service) --> we need .then
    this.messageService
      .sendMessage(this.username, this.messageContent)
      .then(() => {
        // Our Message thread is handling the response so all we need to do is reset the form
        this.messageForm?.reset();
      });
  }
}
