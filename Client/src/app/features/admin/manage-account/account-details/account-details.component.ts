import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../../core/services/admin.service';
import { UserDetail } from '../../../../shared/models/user';
import { CommonModule } from '@angular/common';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-account-details',
  standalone: true,
  imports: [CommonModule, TableModule, TagModule],
  templateUrl: './account-details.component.html',
  styleUrl: './account-details.component.css'
})
export class AccountDetailsComponent implements OnInit{
  userDetail: UserDetail;

  constructor(private adminService: AdminService, private config: DynamicDialogConfig) { }3

  ngOnInit(): void {
    this.userDetail = this.config.data;
    console.log(this.userDetail);
    
  }
}
