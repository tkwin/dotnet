import { Component, Input, signal } from '@angular/core';
import { Register } from "../account/register/register";
import { CommonModule } from '@angular/common';
import { User } from '../../types/user';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, Register],
  templateUrl: './home.html',
  styleUrl: './home.css'
})

export class Home 
{ 
  protected registerMode = signal(false);

  showRegister(value : boolean) {
     this.registerMode.set(value);
  }
}
