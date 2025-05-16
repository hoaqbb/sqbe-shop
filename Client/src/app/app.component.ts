import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountService } from './core/services/account.service';
import { ShopService } from './core/services/shop.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  constructor(
    private accountService: AccountService,
    private shopService: ShopService
  ) {}

  ngOnInit(): void {
    this.initApp();
  }

  initApp() {
    this.shopService.getCategories().subscribe();
    this.shopService.getColors().subscribe();
    this.shopService.getSizes().subscribe();
  }
}
