import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../core/services/account.service';
import { Router, RouterLink } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { environment } from '../../../../environments/environment.development';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { ToastrService } from 'ngx-toastr';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
declare var google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    TextInputComponent,
    RouterLink,
    ConfirmDialogModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  providers: [ConfirmationService],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  validationErrors: string[] = [];
  googleClientId = environment.googleClientId;
  isLogin: boolean = true;
  forgotPasswordForm: FormGroup;

  constructor(
    private accountService: AccountService,
    private router: Router,
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.initializeLoginForm();
    this.initializeForgotPasswordForms();
    this.initializeGoogleBtn();
  }

  initializeLoginForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  initializeForgotPasswordForms() {
    this.forgotPasswordForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  login() {
    this.validationErrors = [];
    if (this.loginForm.invalid) {
      return null;
    }

    return this.accountService.login(this.loginForm.value).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (error) => {
        if (error.status === 403) {
          this.confirmationService.confirm({
            message: 'Vui lòng xác thực email để kích hoạt tài khoản.',
            header: 'Thông báo',
            icon: 'pi pi-info-circle',
            acceptVisible: true,
            rejectVisible: true,
            acceptLabel: 'Kích hoạt',
            rejectLabel: 'Hủy',
            acceptIcon: 'none',
            rejectIcon: 'none',
            acceptButtonStyleClass: 'accept-btn-dialog',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => {
              this.sendVerificationEmail();
            },
          });
        } else {
          this.validationErrors.push(error.error);
        }
      },
    });
  }

  initializeGoogleBtn() {
    google.accounts.id.initialize({
      client_id: this.googleClientId,
      callback: (res: any) => {
        this.accountService.signInWithGoogle(res.credential).subscribe({
          next: () => {
            this.router.navigateByUrl('/');
          },
          error: (err) => {
            this.toastr.error(err.error);
          },
        });
      },
    });

    google.accounts.id.renderButton(document.getElementById('google-btn'), {
      size: 'large',
      shape: 'square',
      text: 'signin_with',
    });
  }

  manageForm() {
    this.isLogin = !this.isLogin;
  }

  sendVerificationEmail() {
    const email = this.loginForm.get('email').value;

    this.accountService.sendVerificationEmail(email).subscribe({
      next: (res: any) => {
        this.toastr.success(res);
      }
    });
  }

  sendResetPasswordEmail() {
    const email = this.forgotPasswordForm.get('email').value;

    this.accountService.sendResetPasswordEmail(email).subscribe();
  }
}
