import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../core/services/account.service';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { environment } from '../../../../environments/environment.development';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { ToastrService } from 'ngx-toastr';
declare var google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, TextInputComponent, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  validationErrors: string[] = [];
  googleClientId = environment.googleClientId;

  constructor(
    private accountService: AccountService,
    private router: Router,
    private formBuilder: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.initializeGoogleBtn();
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  login() {
    this.validationErrors = [];
    if (this.loginForm.invalid) {
      return null;
    }

    return this.accountService.login(this.loginForm.value).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (error) => (this.validationErrors.push(error.error)),
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
}
