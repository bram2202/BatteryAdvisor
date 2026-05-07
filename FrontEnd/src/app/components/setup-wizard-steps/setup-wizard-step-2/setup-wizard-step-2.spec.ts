import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetupWizardStep2 } from './setup-wizard-step-2';

describe('SetupWizardStep2', () => {
  let component: SetupWizardStep2;
  let fixture: ComponentFixture<SetupWizardStep2>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SetupWizardStep2],
    }).compileComponents();

    fixture = TestBed.createComponent(SetupWizardStep2);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
