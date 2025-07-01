import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Size } from '../../models/size';

@Component({
  selector: 'app-size-checkbox-group',
  standalone: true,
  imports: [],
  providers: [
        {
          provide: NG_VALUE_ACCESSOR,
          useExisting: forwardRef(() => SizeCheckboxGroupComponent),
          multi: true
        }
      ],
  templateUrl: './size-checkbox-group.component.html',
  styleUrl: './size-checkbox-group.component.css'
})
export class SizeCheckboxGroupComponent implements ControlValueAccessor{
  @Input() sizes: any[] = [];

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

  toggleSize(id: number, checked: boolean): void {
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
