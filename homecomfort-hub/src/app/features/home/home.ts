import { Component } from '@angular/core';
import { Hero } from './components/hero/hero';
import { FeaturedProducts } from './components/hero/featured-products/featured-products';
import { CategoryGrid } from '../categories/category-grid/category-grid';

@Component({
  selector: 'app-home',
  imports: [Hero,FeaturedProducts,CategoryGrid],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {

}
