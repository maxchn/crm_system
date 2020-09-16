import { Component, OnInit, ViewChild } from '@angular/core';
import { MatStepper } from '@angular/material/stepper';
import { Router } from '@angular/router';

@Component({
  selector: 'app-extended-registration',
  templateUrl: './extended-registration.component.html',
  styleUrls: ['./extended-registration.component.css']
})
export class ExtendedRegistrationComponent implements OnInit {

  @ViewChild('stepper', null) stepper: MatStepper;

  constructor(private _router: Router) { }

  ngOnInit() {

  }

  changeProfile(isSuccessfully: boolean) {
    this.stepper.selected.completed = isSuccessfully;

    if (isSuccessfully) {
      this.stepper.next();
    }
  }

  goToDashboard() {
    this._router.navigate(['/dashboard']);
  }
}