import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PowerCostSetup } from './power-cost-setup';

describe('PowerCostSetup', () => {
  let component: PowerCostSetup;
  let fixture: ComponentFixture<PowerCostSetup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PowerCostSetup],
    }).compileComponents();

    fixture = TestBed.createComponent(PowerCostSetup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
