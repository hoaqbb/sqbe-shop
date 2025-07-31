/// <reference types="cypress" />
// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
//
declare namespace Cypress {
  interface Chainable {
    login(email: string, password: string);
    register(
      lastname: string,
      firstname: string,
      dateOfBirth: string,
      email: string,
      password: string,
      confirmPassword: string
    );
    addToCart();
    fillCheckoutForm(
      fullname: string,
      email: string,
      phoneNumber: string,
      street: string,
      province: string,
      district: string,
      ward: string,
      note: string,
      paymentMethod: string
    );
  }
}

Cypress.Commands.add('login', (email, password) => {
  cy.visit('account/login');

  cy.get('#email').type(email);
  cy.get('#password').type(password);

  cy.get('.login-btn').click();
});

Cypress.Commands.add(
  'register',
  (lastname, firstname, dateOfBirth, email, password, confirmPassword) => {
    cy.visit('account/register');

    cy.get('#lastName').type(lastname);
    cy.get('#firstName').type(firstname);
    cy.get('input#dateOfBirth').type(dateOfBirth);
    cy.get('input').eq(0).click();
    cy.get('#email').type(email);
    cy.get('#password').type(password);
    cy.get('#confirmPassword').type(confirmPassword);

    cy.get('.signup-btn').click();
  }
);

Cypress.Commands.add('addToCart', () => {
  // cy.visit('/categories/all');
  cy.visit('/categories/bag');

  // cy.get('.card.mb-4').first().click();
  cy.get('app-product-item>div.card.mb-4').eq(1).click();
  cy.get('#add-to-cart').click();
  cy.get('app-cart-sidebar').should('be.visible');
});

Cypress.Commands.add(
  'fillCheckoutForm',
  (
    fullname,
    email,
    phoneNumber,
    street,
    province,
    district,
    ward,
    note,
    paymentMethod
  ) => {
    cy.visit('/checkout');

    cy.get('#fullname').type(fullname);
    cy.get('#email').type(email);
    cy.get('#phoneNumber').type(phoneNumber);
    cy.get('#street').type(street);
    cy.get('#province').select(province);
    cy.get('#district').select(district);
    cy.get('#ward').select(ward);
    cy.get('#note').type(note);
    cy.get('[type="radio"]').check(paymentMethod)
  }
);
