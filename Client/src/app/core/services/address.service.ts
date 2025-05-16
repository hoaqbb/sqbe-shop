import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  url = environment.addressApiUrl;

  async getProvinces() {
    return (await fetch(this.url + '1/0.htm')).json();
  }

  async getDistricts(provinceId: string) {
    return (await fetch(this.url + '2/' + provinceId + '.htm')).json();
  }

  async getWards(districtId: string) {
    return (await fetch(this.url + '3/' + districtId + '.htm')).json();
  }

  async getFullAddress(wardId: string) {
    return (await fetch(this.url + '5/' + wardId + '.htm')).json();
  }
}
