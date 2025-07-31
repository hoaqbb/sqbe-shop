import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-shop-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, ChartModule, TableModule, TagModule],
  templateUrl: './shop-dashboard.component.html',
  styleUrl: './shop-dashboard.component.css',
})
export class ShopDashboardComponent implements OnInit {
  overviewData: any;
  revenueData: any;

  chartData: any;
  chartOptions: any;
  pieChartData: any;
  pieChartOptions: any;
  selectedYear: number;

  constructor(private adminService: AdminService) {}
  ngOnInit(): void {
    this.getOverview();
    this.getRevenueChart();
  }

  getOverview() {
    this.adminService.getOverview().subscribe((res: any) => {
      this.overviewData = res;
      this.pieChartInit();
    });
  }

  getRevenueChart() {
    this.adminService.getRevenue().subscribe((res: any) => {
      this.revenueData = res;
      this.selectedYear = this.revenueData.years.at(0); //get first element of array
      this.chartInit();
    });
  }

  getRevenueChartWithYear(year: number) {
    this.adminService.getRevenueWithYear(year).subscribe((res: any) => {
      this.revenueData = res;
      this.selectedYear = year;
      this.chartInit();
    });
  }

  //Doanh thu từng tháng theo năm
  chartInit() {
    const documentStyle = getComputedStyle(document.documentElement);

    const months = this.revenueData.revenues.map((x) => x.month.toString());
    const revenue = this.revenueData.revenues.map((x) => x.revenue);
    const totalOrder = this.revenueData.revenues.map((x) => x.totalOrder);

    this.chartData = {
      labels: months,
      datasets: [
        {
          label: 'Doanh thu',
          data: revenue,
          backgroundColor: documentStyle.getPropertyValue('--teal-400'),
        },
        {
          label: 'Đơn hàng',
          data: totalOrder,
          backgroundColor: documentStyle.getPropertyValue('--red-400'),
        },
      ],
    };

    this.chartOptions = {
      maintainAspectRatio: false,
      aspectRatio: 0.8,
    };
  }

  //Doanh số theo danh mục
  pieChartInit() {
    const categories = this.overviewData.categorySales.map(
      (x) => x.categoryName
    );
    const data = this.overviewData.categorySales.map((x) => x.totalSold);

    this.pieChartData = {
      labels: categories,
      datasets: [
        {
          data: data,
        },
      ],
    };

    this.pieChartOptions = {
      responsive: true,
      cutout: '60%',
    };
  }
}
