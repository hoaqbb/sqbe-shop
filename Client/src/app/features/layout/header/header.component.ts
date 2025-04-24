import {
  Component,
  ElementRef,
  HostListener,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { ShopService } from '../../../core/services/shop.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  @ViewChild('navBar') navBar: ElementRef;

  constructor(
    public accountService: AccountService,
    private renderer: Renderer2,
    public shopService: ShopService
  ) {}

  @HostListener('window:scroll', [])
  ngAfterViewInit(): void {
    if (window.pageYOffset > 50) {
      // Điều kiện cuộn
      this.renderer.addClass(this.navBar.nativeElement, 'nav-scroll');
    } else {
      this.renderer.removeClass(this.navBar.nativeElement, 'nav-scroll');
    }
  }
}
