import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TabViewModule } from 'primeng/tabview';
import { ManageColorComponent } from "./manage-color/manage-color.component";
import { ManageSizeComponent } from "./manage-size/manage-size.component";

@Component({
  selector: 'app-manage-variant',
  standalone: true,
  imports: [
    CommonModule,
    TabViewModule,
    ManageColorComponent,
    ManageSizeComponent
],
  templateUrl: './manage-variant.component.html',
  styleUrl: './manage-variant.component.css'
})
export class ManageVariantComponent {
}
