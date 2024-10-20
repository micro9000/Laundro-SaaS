export interface OnboardingFormValues {
  tenantName: string;
  companyAddress: string;
  websiteUrl?: string;
  businessRegistrationNumber?: string;
  primaryContactName: string;
  contactEmail: string;
  phoneNumber: string;
  storeName: string;
  storeLocation: string;
}

interface FormInputProperty {
  label: string;
  description: string;
}

export const FormInputProperties = {
  tenantName: {
    label: 'Tenant Name',
    description: 'Your company name',
  } as FormInputProperty,
};
