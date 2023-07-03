import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css'],
})
export class RolesModalComponent implements OnInit {
  // Three pieces the Modal Component is looking for
  username = '';
  availableRoles: any[] = [];
  selectedRoles: any[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  // Checkbox
  updateChecked(checkedValue: string) {
    // Seeing if its inside the "selectedRoles" array
    const index = this.selectedRoles.indexOf(checkedValue);

    // If the index is not negative 1 (or 0 or above meaning its checked)
    index != -1
      ? this.selectedRoles.splice(index, 1)
      : this.selectedRoles.push(checkedValue);
  }
}
