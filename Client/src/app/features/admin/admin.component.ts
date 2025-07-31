import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AdminService } from '../../core/services/admin.service';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit{
  userFullname: string;

  constructor(private adminService: AdminService, private accountService: AccountService) {}

  ngOnInit(): void {
    this.initAdminPage();
    this.getUserFullname();
  }

  initAdminPage() {
    this.loadScript('assets/js/sb-admin-2.min.js');
    this.adminService.getCategories().subscribe();
    this.adminService.getColors().subscribe();
    this.adminService.getSizes().subscribe();
  }

  loadScript(scriptUrl: string) {
    const script = document.createElement('script');
    script.src = scriptUrl;
    script.async = true;
    document.body.appendChild(script);
  }

  getCategories() {
    this.adminService.getCategories().subscribe({
      next: (response: any) => {},
      error: (error: any) => console.log(error)
    })
  }

  getUserFullname() {
    this.userFullname = this.accountService.currentUser().lastname + ' ' + this.accountService.currentUser().firstname;
  }
}
