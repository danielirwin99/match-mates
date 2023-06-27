import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css'],
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label = ""
  @Input() maxDate: Date | undefined
  bsConfig: Partial<BsDatepickerConfig> | undefined

  // Binds a FormControl to a DOM element
  constructor(@Self() public ngControl: NgControl) {
    // Letting it equal this --> TextInputClass
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: "theme-green",
      dateInputFormat: "DD MMMM YYYY"
    }
  }

  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}

  // To get around Typescript strict mode
  // Get --> when we try to access this control --> It will go and get it and return the line below
  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}
