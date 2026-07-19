import { Component, OnInit } from '@angular/core';
import { Product } from '../../../../../shared/models/product.model';
import { ApiService } from '../../../../../core/services/api.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-featured-products',
  imports: [CommonModule,RouterLink],
  templateUrl: './featured-products.html',
  styleUrl: './featured-products.css',
})
export class FeaturedProducts  implements OnInit { 
  products: Product[] = [];
   isLoading = true; 
   error: string | null = null; 
   constructor(private apiService: ApiService) {} 

   ngOnInit() {
     this.loadProducts(); 
    } 
    
    loadProducts() { 
      this.apiService.getProducts().subscribe({
         next: (data) => {
           this.products = data.slice(0, 6); 
           this.isLoading = false; 
          }, 
          error: (err) => { 
            this.error = 'Failed to load products'; 
            this.isLoading = false; 
          } });
         }

}
