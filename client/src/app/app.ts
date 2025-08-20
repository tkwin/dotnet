import { Component, inject } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, Nav, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App
{  
  protected router = inject(Router);
}
