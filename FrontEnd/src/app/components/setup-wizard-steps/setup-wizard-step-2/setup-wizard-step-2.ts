import { AfterViewInit, Component, ElementRef, inject, OnDestroy, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ApiEntityService } from '../../../../services/api-services/api-entity-service/api-entity-service';
import { EntityDto } from '../../../../models/entity-dto';
import { MultiSelectModule } from 'primeng/multiselect';

@Component({
  selector: 'app-setup-wizard-step-2',
  standalone: true,
  imports: [InputTextModule, FormsModule, ButtonModule, MultiSelectModule],
  templateUrl: './setup-wizard-step-2.html',
  styleUrl: './setup-wizard-step-2.scss',
})
export class SetupWizardStep2 implements AfterViewInit, OnDestroy {
  private readonly apiEntityService = inject(ApiEntityService);

  public entities: EntityDto[] = [];
  public selectedPowerConsumptionEntities: EntityDto[] = [];
  public selectedPowerProductionEntities: EntityDto[] = [];
  public selectedPvEntities: EntityDto[] = [];

  // Element we observe to know when this step enters the viewport.
  @ViewChild('visibilityTarget', { static: true })
  private readonly visibilityTarget!: ElementRef<HTMLElement>;

  private intersectionObserver: IntersectionObserver | null = null;
  // Prevents calling the visibility handler more than once.
  private hasTriggeredVisibility = false;

  ngAfterViewInit(): void {
    // Observe viewport intersection after the template is rendered.
    this.intersectionObserver = new IntersectionObserver((entries) => {
      for (const entry of entries) {
        if (!this.hasTriggeredVisibility && entry.isIntersecting) {
          this.hasTriggeredVisibility = true;
          this.loadEntities();
        }
      }
    });

    this.intersectionObserver.observe(this.visibilityTarget.nativeElement);
  }

  ngOnDestroy(): void {
    // Clean up observer to avoid leaks when the component is destroyed.
    this.intersectionObserver?.disconnect();
    this.intersectionObserver = null;
  }

  loadEntities(): void {
    this.apiEntityService.getEntities().then((entities) => {
      console.log('Loaded entities:', entities);
      this.entities = entities;
    });
  }
}
