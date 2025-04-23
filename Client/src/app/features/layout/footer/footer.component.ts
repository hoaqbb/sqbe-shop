import { Component } from '@angular/core';
import { MarqueeComponent } from "../marquee/marquee.component";

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [MarqueeComponent],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {

}
