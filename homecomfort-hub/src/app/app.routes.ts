import { Routes } from '@angular/router';

export const routes: Routes = [
     {
    path: '',
    loadComponent: () => import('./features/home/home').then(m => m.Home)
   },
  {
    path: 'products',
    loadComponent: () => import('./features/product/productlist/productlist').then(m => m.Productlist)
  },
  {
  path: 'products/add',
  loadComponent: () => import('./features/product/add-product/add-product').then(m => m.AddProduct)
},
  {
    path: 'products/:id',
    loadComponent: () => import('./features/product/productdetail/productdetail').then(m => m.Productdetail)
  },
  {
    path: 'categories',
    loadComponent: () => import('./features/categories/category-grid/category-grid').then(m => m.CategoryGrid)
  },
  {
  path: 'categories/add',
  loadComponent: () => import('./features/categories/add-category/add-category').then(m => m.AddCategory)
},
  {
    path: 'categories/:id',
    loadComponent: () => import('./features/categories/category-detail/category-detail').then(m => m.CategoryDetail)
  },
  

//   {
//     path: 'blog',
//     loadComponent: () => import('./features/blog/blog-list/blog-list.component').then(m => m.BlogListComponent)
//   },
//   {
//     path: 'blog/:id',
//     loadComponent: () => import('./features/blog/blog-detail/blog-detail.component').then(m => m.BlogDetailComponent)
//   },
//   {
//     path: 'about',
//     loadComponent: () => import('./features/about/about.component').then(m => m.AboutComponent)
//   },
  {
    path: '**',
    redirectTo: ''
  }
];
