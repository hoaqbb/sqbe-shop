import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Color } from '../../models/color';

@Component({
  selector: 'app-color-checkbox-group',
  standalone: true,
  imports: [CommonModule],
  providers: [
      {
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => ColorCheckboxGroupComponent),
        multi: true
      }
    ],
  templateUrl: './color-checkbox-group.component.html',
  styleUrl: './color-checkbox-group.component.css'
})
export class ColorCheckboxGroupComponent implements ControlValueAccessor{
  @Input() colors: any[] = [];

  selectedIds: number[] = [];

  onChange = (_: any) => {};
  onTouched = () => {};

  writeValue(value: number[]): void {
    this.selectedIds = value || [];
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  toggleColor(id: number, checked: boolean): void {
    const updated = [...this.selectedIds];
    if (checked && !updated.includes(id)) {
      updated.push(id);
    } else if (!checked) {
      const index = updated.indexOf(id);
      if (index !== -1) updated.splice(index, 1);
    }

    this.selectedIds = updated;
    this.onChange(this.selectedIds);
  }
}
