import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AccountService } from './core/services/account.service';
import { User } from './shared/models/user';
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
    this.setCurrentUser();
    this.getCategories();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));

    if (user) {
      this.accountService.currentUser.set(user);
    }
  }

  getCategories() {
    this.shopService.getCategories().subscribe();
  }
}
