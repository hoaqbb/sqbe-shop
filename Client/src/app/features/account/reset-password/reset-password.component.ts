import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../../../core/services/account.service';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { ActivatedRoute } from '@angular/router';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TextInputComponent],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent implements OnInit {
  resetPassForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private accountService: AccountService,
    private formBuilder: FormBuilder
  ) {}

  httpParam = new HttpParams();
  ngOnInit(): void {
    this.initializeForm();
    this.route.queryParams.subscribe((params: HttpParams) => {
      if (params['token']) {
        Object.keys(params).forEach((key) => {
          this.httpParam = this.httpParam.append(key, params[key]);
        });
      }
    });
  }

  initializeForm() {
    this.resetPassForm = this.formBuilder.group({
      password: ['', [Validators.required, Validators.minLength(10)]],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    this.resetPassForm.controls?.['password'].valueChanges.subscribe(() => {
      this.resetPassForm.controls?.['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null
        : { isMatching: true };
    };
  }

  updatePass() {
    const newPass = this.resetPassForm.get('password').value;
    this.accountService.sendNewPass(newPass, this.httpParam).subscribe();
  }
}
