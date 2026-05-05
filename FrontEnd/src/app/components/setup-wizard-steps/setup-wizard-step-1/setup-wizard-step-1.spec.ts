import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetupWizardStep1 } from './setup-wizard-step-1';

describe('SetupWizardStep1', () => {
  let component: SetupWizardStep1;
  let fixture: ComponentFixture<SetupWizardStep1>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SetupWizardStep1],
    }).compileComponents();

    fixture = TestBed.createComponent(SetupWizardStep1);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
