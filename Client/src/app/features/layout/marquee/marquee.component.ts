import { Component } from '@angular/core';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-marquee',
  standalone: true,
  imports: [],
  templateUrl: './marquee.component.html',
  styleUrl: './marquee.component.css'
})
export class MarqueeComponent {
  marquee = environment.marquee;
  fixedMarquee = environment.fixedMarquee;
}
