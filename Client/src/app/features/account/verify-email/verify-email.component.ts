import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './verify-email.component.html',
  styleUrl: './verify-email.component.css',
})
export class VerifyEmailComponent implements OnInit {
  isSuccess: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params: HttpParams) => {
      if (params['token']) {
        let httpParams = new HttpParams();
        Object.keys(params).forEach((key) => {
          httpParams = httpParams.append(key, params[key]);
        });
        this.accountService.verifyAccount(httpParams).subscribe({
          next: (res: any) => {
            this.isSuccess = true;
          },
          error: (err) => {
            this.isSuccess = false;
            console.log(err);
          },
        });
      }
    });
  }
}
