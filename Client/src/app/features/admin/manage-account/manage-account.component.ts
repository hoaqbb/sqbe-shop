import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { User, UserDetail } from '../../../shared/models/user';
import { Pagination } from '../../../shared/models/pagination';
import { AdminService } from '../../../core/services/admin.service';
import { TagModule } from 'primeng/tag';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AccountDetailsComponent } from './account-details/account-details.component';

@Component({
  selector: 'app-manage-account',
  standalone: true,
  imports: [CommonModule, TableModule, TagModule],
  templateUrl: './manage-account.component.html',
  styleUrl: './manage-account.component.css',
  providers: [DialogService],
})
export class ManageAccountComponent implements OnInit {
  ref: DynamicDialogRef | undefined;
  paginatedAccounts: Pagination<User>;

  constructor(private adminService: AdminService, private dialogService: DialogService,) {}

  ngOnInit(): void {
    this.getAccounts();
  }

  getAccounts() {
    this.adminService.getAccounts().subscribe({
      next: (response: any) => this.paginatedAccounts = response,
      error: (error) => console.error('Error fetching accounts:', error)
    })
  }

  getAccountDetail(id) {
      this.adminService
        .getAccountInfo(id)
        .subscribe((response: UserDetail) => {
          this.ref = this.dialogService.open(AccountDetailsComponent, {
            data: response,
            modal: true
          });
        });
    }
}
