describe('Manage Product Function Tests', () => {
    it('Truy cập trang admin thất bại với tài khoản không phải admin', () => {
      cy.login('user@example.com', '123456789');
      cy.wait(500);
      cy.getCookie('accessToken').should('exist');
      cy.getCookie('refreshToken').should('exist');
      cy.visit('/admin/product');

      cy.url().should('include', 'not-found');
    });

  context('Người dùng đăng nhập vào hệ thống với tài khoản admin', () => {
    beforeEach(() => {
      cy.login('admin@gmail.com', 'P@ssWord123');
      cy.wait(500);
      cy.getCookie('accessToken').should('exist');
      cy.getCookie('refreshToken').should('exist');
      cy.visit('/admin/product');
      cy.url().should('include', '/admin/product');
    });

    it('Xem danh sách sản phẩm và lọc sản phẩm', () => {
        cy.get('.filter-btn').click();
        cy.get('select#sort').select('GIÁ GIẢM DẦN');
        cy.get('select#category').select('TOP');
    cy.contains('label.size-option', 'S').find('input[type="checkbox"]').check({ force: true });
    cy.contains('label.size-option', 'M').find('input[type="checkbox"]').check({ force: true });
    cy.contains('label.size-option', 'L').find('input[type="checkbox"]').check({ force: true });

    cy.contains('label.colour-option', 'Đen').find('input[type="checkbox"]').check({ force: true });
    cy.contains('label.colour-option', 'Nâu').find('input[type="checkbox"]').check({ force: true });

        cy.get('.apply-btn').click();
    });

    it('Thêm sản phẩm mới', () => {
      cy.get('.btn-success').click();
      cy.get('#name').type('AUTOMATION TEST CREATE PRODUCT');
      cy.get('#price').type('350000');
      cy.get('#discount').type('5');
      cy.get('#description').type('*Mô tả ngắn cho sản phẩm');
      cy.get('#category').select('Top');
      cy.contains('label.colour-option', 'Xanh lá')
        .find('input[type="checkbox"]')
        .check({ force: true });
      cy.contains('label.colour-option', 'Hồng')
        .find('input[type="checkbox"]')
        .check({ force: true });

      cy.contains('label.size-option', 'M')
        .find('input[type="checkbox"]')
        .check({ force: true });
      cy.contains('label.size-option', 'L')
        .find('input[type="checkbox"]')
        .check({ force: true });
      cy.contains('label.size-option', 'XL')
        .find('input[type="checkbox"]')
        .check({ force: true });

      cy.get('p-fileupload#mainImage input[type="file"]')
        .first()
        .then(($input) => {
          $input[0].removeAttribute('style');
        });
      cy.get('p-fileupload#mainImage input[type="file"]')
        .first()
        .selectFile('cypress/fixtures/sample1.jpg');

      cy.get('p-fileupload#subImage input[type="file"]')
        .first()
        .then(($input) => {
          $input[0].removeAttribute('style');
        });
      cy.get('p-fileupload#subImage input[type="file"]')
        .first()
        .selectFile('cypress/fixtures/sample2.jpg');

      cy.get('p-fileupload#otherImages input[type="file"]')
        .first()
        .then(($input) => {
          $input[0].removeAttribute('style');
        });
      cy.get('p-fileupload#otherImages input[type="file"]')
        .first()
        .selectFile('cypress/fixtures/sample3.jpg');

      cy.intercept('https://localhost:7100/api/Products').as('createProduct');
      cy.get('.create-btn').click();
      cy.wait('@createProduct').then((interception) => {
        expect(interception.response?.statusCode).to.eq(201);
      });
      cy.contains('Thêm sản phẩm thành công!').should('be.visible');
    });

    it('Chỉnh sửa thông tin sản phẩm', () => {
        cy.get('table>tbody>tr>td>p-button#update-btn').first().click();
        cy.get('#name').clear().type('AUTOMATION TEST UPDATE PRODUCT');
        cy.get('#price').clear().type('200000');
        cy.get('#discount').clear().type('0');
        cy.get('#description').clear().type('*Chỉnh sửa mô tả sản phẩm');
        cy.get('#category').select('Outerwear');
        cy.intercept('https://localhost:7100/api/Products').as('updateProduct');
        cy.get('.save-btn').click();
        cy.contains('Cập nhật sản phẩm thành công!').should('be.visible');
    });

    it('Xóa sản phẩm', () => {
        cy.get('table>tbody>tr>td>p-button#delete-btn').first().click();
        cy.get('button.p-confirm-dialog-accept').click();
        cy.contains('Xóa sản phẩm thành công!').should('be.visible');
    });
  });
});
