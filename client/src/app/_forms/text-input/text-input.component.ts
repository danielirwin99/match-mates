import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css'],
})
// ControlValueAccessor --> Bridge between API Forms and native element DOM
export class TextInputComponent implements ControlValueAccessor {
  // This is for passing other bits of data to our inputs
  @Input() label = '';
  @Input() type = 'text';

  // Binds a FormControl to a DOM element
  constructor(@Self() public ngControl: NgControl) {
    // Letting it equal this --> TextInputClass
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}

  // To get around Typescript strict mode
  // Get --> when we try to access this control --> It will go and get it and return the line below
  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}
